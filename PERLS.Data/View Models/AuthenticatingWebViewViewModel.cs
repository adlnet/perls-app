using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Analytics;
using Float.Core.L10n;
using Float.Core.Net;
using PERLS.Data.Definition.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Authenticating web view view model.
    /// </summary>
    public abstract class AuthenticatingWebViewViewModel : BasePageViewModel
    {
        Task prepareAuthenticatingUrlTask;

        Uri initialUrl;
        Uri currentUrl;
        IDocumentDecider documentDecider = DependencyService.Get<IDocumentDecider>();

        /// <summary>
        /// Indicates whether the originally requested destination has successfully been loaded.
        /// </summary>
        bool didLoadDestination;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatingWebViewViewModel"/> class.
        /// </summary>
        /// <param name="linkClicked">Link clicked.</param>
        /// <param name="pageFailedToLoad">Page failed to load.</param>
        /// <param name="destinationPath">Destination path.</param>
        /// <param name="refreshOnAppear">If the webview should refresh on appearing or re-appearing.</param>
        /// <param name="networkConnectionService">The network connection service.</param>
        protected AuthenticatingWebViewViewModel(ICommand linkClicked, ICommand pageFailedToLoad, string destinationPath, bool refreshOnAppear = false, INetworkConnectionService networkConnectionService = null)
        {
            Destination = new Uri(destinationPath, UriKind.RelativeOrAbsolute);
            NavigateCommand = linkClicked;
            HandleErrorCommand = pageFailedToLoad;
            LoadingEvent = new Command<WebNavigationEventArgs>(HandleLoadEvent);
            RefreshOnAppear = refreshOnAppear;
            NetworkConnectionService = networkConnectionService ?? DependencyService.Get<INetworkConnectionService>();

            Refresh();
        }

        /// <summary>
        /// Gets the loading event for the WebView.
        /// </summary>
        /// <value>The loading event.</value>
        public ICommand LoadingEvent { get; }

        /// <summary>
        /// Gets a value indicating whether the webview should refresh when the view re-appears.
        /// </summary>
        /// <value>
        /// A value indicating whether the webview should refresh when the view re-appears.
        /// </value>
        public bool RefreshOnAppear { get; }

        /// <summary>
        /// Gets or sets the initial absolute URL for the webview.
        /// </summary>
        /// <remarks>
        /// This is the initial URL loaded into the webview--it may not reflect the current URL.
        /// For the current URL, use <see cref="CurrentUrl"/>.
        /// </remarks>
        /// <value>The initial URL.</value>
        public Uri InitialUrl
        {
            get => initialUrl;
            protected set => SetField(ref initialUrl, value);
        }

        /// <summary>
        /// Gets or sets the current Url loaded in the webview.
        /// </summary>
        /// <value>The current URL.</value>
        public Uri CurrentUrl
        {
            get => currentUrl;
            protected set => SetField(ref currentUrl, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page should display an error message.
        /// </summary>
        /// <value>
        /// A boolean indicating whether the page should display an error message.
        /// </value>
        public bool DisplayErrorMessage { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the requested resource requires authentication.
        /// </summary>
        /// <remarks>By default, external requests do not require authentication while internal requests do.</remarks>
        /// <value><c>true</c> if authentication is required, <c>false</c> otherwise.</value>
        protected virtual bool RequiresAuthentication => IsDestinationExternal == false;

        /// <summary>
        /// Gets the original target destination for the webview.
        /// </summary>
        /// <value>Possibly either a relative or absolute uri representing the original destination for the webview.</value>
        protected virtual Uri Destination { get; }

        /// <summary>
        /// Gets the command for navigating to a new piece of content.
        /// </summary>
        /// <value>The navigating command.</value>
        protected virtual ICommand NavigateCommand { get; }

        /// <summary>
        /// Gets the command for responding to an error.
        /// </summary>
        /// <value>The error command.</value>
        protected virtual ICommand HandleErrorCommand { get; }

        /// <summary>
        /// Gets the base URI used by the app for loading resources (e.g. API requests).
        /// </summary>
        /// <value>The base URI.</value>
        protected Uri BaseUri => NetworkConnectionService.BaseUri;

        /// <summary>
        /// Gets the current network connection service.
        /// </summary>
        /// <value>The current network connection service.</value>
        protected INetworkConnectionService NetworkConnectionService { get; }

        /// <summary>
        /// Gets a value indicating whether the destination for the web view is external.
        /// </summary>
        /// <remarks>
        /// A destination is considered external when it has a host and that host does not match
        /// the current base URI used for API requests.
        /// </remarks>
        /// <value><c>true</c> if the destination is external, <c>false</c> otherwise.</value>
        protected virtual bool IsDestinationExternal => !string.IsNullOrEmpty(Destination.Host) && Destination.Host != BaseUri.Host;

        /// <summary>
        /// Gets the URL which authenticates the user and redirects to the requested resource.
        /// </summary>
        /// <returns>The authenticating URL.</returns>
        public async Task<Uri> GetAuthenticatingUrl()
        {
            await prepareAuthenticatingUrlTask.ConfigureAwait(false);
            return InitialUrl;
        }

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();
            InitialUrl = null;
            didLoadDestination = false;
            prepareAuthenticatingUrlTask = Task.Run(DetermineInitialUrl);
        }

        /// <summary>
        /// Handles changes in the navigation state by the Webview object.
        /// </summary>
        /// <param name="eventArgs">The navigation event.</param>
        protected virtual void HandleLoadEvent(WebNavigationEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                return;
            }

            switch (eventArgs)
            {
                case WebNavigatingEventArgs args:
                    // Every case is a special case if you love it enough.
                    if (eventArgs.Url.StartsWith("blob:", StringComparison.InvariantCulture))
                    {
                        return;
                    }

                    var originUri = CurrentUrl;

                    // at this point, the URL can reasonably be expected to be valid
                    // but in the rare case where it is not, we want to know about it
                    if (!Uri.TryCreate(eventArgs.Url, UriKind.RelativeOrAbsolute, out var targetUri))
                    {
                        DependencyService.Get<AnalyticsService>().TrackEvent("invalid_navigating_url", new Dictionary<string, string>
                        {
                            ["url"] = eventArgs.Url,
                        });

                        return;
                    }

                    // If we are returning to our initial Url after it has loaded, we don't want to reload the page, so we just cancel instead.
                    // We only need to do this for iOS due to differences in how Page Appearing/Disappearing works between platforms.
                    if (Device.RuntimePlatform == Device.iOS && targetUri == InitialUrl && didLoadDestination)
                    {
                        args.Cancel = true;
                        IsLoading = false;
                        return;
                    }

                    // If the originally requested destination hasn't successfully loaded yet,
                    // then allow the request to proceed.
                    if (!didLoadDestination)
                    {
                        IsLoading = true;
                        CurrentUrl = targetUri;
                        return;
                    }

                    // SPECIAL CASE! Open documents (like PDF) in document opener.
                    var filepath = documentDecider.GetDocumentFilePath(targetUri);
                    if (filepath != null)
                    {
                        args.Cancel = true;
                        NavigateCommand.Execute(new Uri(filepath));
                        IsLoading = false;
                        return;
                    }

                    // Once the destination has successfully loaded,
                    // see if the new request should be handled in a new window.
                    if (args.NavigationEvent == WebNavigationEvent.NewPage && IsNewRequest(targetUri))
                    {
                        args.Cancel = true;
                        NavigateCommand.Execute(targetUri);
                        IsLoading = false;
                        return;
                    }

                    CurrentUrl = targetUri;

                    // If the only change to the uri is in the fragment, then there's no need to display the loading indicator.
                    // There seems to be a bug in Xamarin Forms where if there is a fragment change,
                    // it will trigger the Navigating event, but never the Navigated event (which would leave the loading indicator on screen).
                    // The Authority check is sort of a work around for filtering out navigating events for youtube videos that never trigger navigated events.
                    if (IsOnlyFragmentChange(originUri, targetUri) || originUri.Authority != targetUri.Authority)
                    {
                        return;
                    }

                    // This is a work around to fix an issue where navigating events are being triggered when an annotation is added that doesn't trigger a corresponding Navigated event.
                    if (targetUri.LocalPath.Contains("/lrs/"))
                    {
                        return;
                    }

                    if (args.NavigationEvent != WebNavigationEvent.Refresh)
                    {
                        IsLoading = true;
                    }

                    break;
                case WebNavigatedEventArgs args:
                    IsLoading = false;

                    if (args.Result != WebNavigationResult.Success && !didLoadDestination)
                    {
                        if (Connectivity.NetworkAccess == NetworkAccess.None && (args.Url == null || !new Uri(args.Url).IsFile))
                        {
                            Error = new HttpConnectionException(Localize.String("NoInternetMessage"));
                            HandleErrorCommand.Execute(null);
                        }
                        else
                        {
                            Error = new HttpConnectionException(Strings.PageLoadingFailedMessage);
                            HandleErrorCommand.Execute(null);
                        }
                    }
                    else if (!args.Url.Contains(Constants.TokenSessionPath))
                    {
                        didLoadDestination = true;
                    }
                    else
                    {
                        // at this point, the URL can reasonably be expected to be valid
                        // but in the rare case where it is not, we want to know about it
                        if (!Uri.TryCreate(eventArgs.Url, UriKind.RelativeOrAbsolute, out var currentUri))
                        {
                            DependencyService.Get<AnalyticsService>().TrackEvent("invalid_navigated_url", new Dictionary<string, string>
                            {
                                ["url"] = eventArgs.Url,
                            });

                            return;
                        }

                        CurrentUrl = currentUri;
                    }

                    break;
                default:
                    IsLoading = false;
                    break;
            }
        }

        /// <summary>
        /// Gets whether a requested URI represents a new request (and thus, should be opened in a new window).
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <returns><c>true</c> if it represents a new request.</returns>
        /// <remarks>
        /// A request is considered "new" if it's path is not similar to the original destination path.
        /// This means that requests to other resources are considered "new", but requests that only change the fragment or subpath are not.
        /// </remarks>
        protected virtual bool IsNewRequest(Uri requestedUri)
        {
            if (requestedUri == null || CurrentUrl == null)
            {
                return false;
            }

            // If the scheme or host is different, then this is a new request.
            if (requestedUri.Scheme != CurrentUrl.Scheme
                || requestedUri.Host != CurrentUrl.Host)
            {
                return true;
            }

            // If the only change is to a different file in the same path,
            // or to a file in a subdirectory of the current path,
            // then this is not a new request.
            var currentDirectory = Path.GetDirectoryName(CurrentUrl.LocalPath);
            var requestedDirectory = Path.GetDirectoryName(requestedUri.LocalPath);

            if (currentDirectory == null || requestedDirectory == null)
            {
                return false;
            }

            if (requestedDirectory == currentDirectory || requestedDirectory.StartsWith(currentDirectory, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            // If the requested path is blank, but does not match the current local path, allow it to open a new page.
            if (requestedUri.LocalPath == "/")
            {
                return !requestedUri.LocalPath.Equals(CurrentUrl.LocalPath);
            }

            return !requestedUri.LocalPath.Contains(CurrentUrl.LocalPath)
                && !CurrentUrl.LocalPath.Contains(requestedUri.LocalPath);
        }

        static bool IsOnlyFragmentChange(Uri originalUri, Uri newUri)
        {
            if (originalUri.Host != newUri.Host || originalUri.PathAndQuery != newUri.PathAndQuery)
            {
                return false;
            }

            if (originalUri.Fragment != newUri.Fragment)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the server end point.
        /// </summary>
        /// <returns>The server end point.</returns>
        async Task DetermineInitialUrl()
        {
            if (InitialUrl != null)
            {
                return;
            }

            if (IsDestinationExternal)
            {
                InitialUrl = Destination;
                return;
            }

            var uriBuilder = new UriBuilder(BaseUri);

            if (RequiresAuthentication)
            {
                if (NetworkConnectionService.AuthStrategy is OAuth2StrategyBase authStrategy)
                {
                    uriBuilder.Path = $"{Constants.TokenSessionPath}{await authStrategy.GetAccessToken().ConfigureAwait(false)}/";
                }

                // If the destination was provided as an absolute URI, remove the scheme, host, and port so we can redirect
                // the request through the start session endpoint.
                var destination = Destination.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Scheme & ~UriComponents.Host & ~UriComponents.Port, UriFormat.SafeUnescaped);
                uriBuilder.Query = Constants.DestinationQuery + Uri.EscapeDataString(destination);
            }
            else
            {
                uriBuilder.Path = Destination.LocalPath;
                uriBuilder.Query = Destination.Query;
            }

            InitialUrl = uriBuilder.Uri;
        }
    }
}
