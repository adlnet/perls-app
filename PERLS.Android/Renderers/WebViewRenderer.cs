using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using PERLS.Components;
using PERLS.Data.Definition.Services;
using PERLS.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(WebView), typeof(PERLS.Droid.Renderers.WebViewRenderer))]

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// Web view renderer.
    /// </summary>
    public class WebViewRenderer : Xamarin.Forms.Platform.Android.WebViewRenderer, Android.Webkit.IDownloadListener
    {
        FullScreenEnabledWebView webView;
        IDocumentDecider documentDecider = DependencyService.Get<IDocumentDecider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewRenderer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public WebViewRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc />
#pragma warning disable CA1054 // Uri parameters should not be strings
        public void OnDownloadStart(string url, string userAgent, string contentDisposition, string mimetype, long contentLength)
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            if (url == null || url.StartsWith("blob:", StringComparison.InvariantCulture))
            {
                return;
            }

            var filepath = documentDecider.GetDocumentFilePath(new Uri(url));

            // SPECIAL CASE! Open documents (like PDF) in document opener.
            if (filepath != null)
            {
                ((App)Application.Current).OpenUri(new Uri(filepath)).ConfigureAwait(false);
                return;
            }

            // if an anchor tag has a "download" attribute, we'll end up here
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri parsed))
            {
                ((App)Application.Current).OpenExternalUri(parsed);
            }
        }

        /// <inheritdoc />
        protected override Android.Webkit.WebViewClient GetWebViewClient()
        {
            return new DefaultWebViewClient(this);
        }

        /// <inheritdoc/>
        protected override FormsWebChromeClient GetFormsWebChromeClient()
        {
            var client = new DefaultWebChromeClient();
            client.EnterFullscreenRequested += OnEnterFullscreenRequested;
            client.ExitFullscreenRequested += OnExitFullscreenRequested;
            return client;
        }

        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            var settings = Control.Settings;

            // Route even requests for a new window to our current client (so we can decide what to do).
            settings.SetSupportMultipleWindows(false);

            if (e == null || Control == null)
            {
                return;
            }

            if (e.OldElement == null)
            {
                settings.UserAgentString = FormattableString.Invariant($"{settings.UserAgentString} {DependencyService.Get<IAppContextService>().UserAgentSuffix}");
            }

            if (e.NewElement is FullScreenEnabledWebView enabledWebView)
            {
                webView = enabledWebView;
            }

            Control.SetDownloadListener(this);
        }

        /// <inheritdoc/>
        protected override void OnVisibilityChanged(Android.Views.View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);

            // we need to manually pause a non-visible page to pause videos, etc.
            switch (visibility)
            {
                case ViewStates.Gone:
                case ViewStates.Invisible:
                    Control.OnPause();
                    break;
                case ViewStates.Visible:
                    Control.OnResume();
                    break;
            }
        }

        /// <summary>
        /// Executes the full-screen command on the <see cref="FullScreenEnabledWebView"/> if available. The
        /// Xamarin view to display in full-screen is sent as a command parameter.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void OnEnterFullscreenRequested(
            object sender,
            EnterFullScreenRequestedEventArgs eventArgs)
        {
            if (webView == null)
            {
                return;
            }

            if (webView.EnterFullScreenCommand != null && webView.EnterFullScreenCommand.CanExecute(null))
            {
                webView.EnterFullScreenCommand.Execute(eventArgs.View.ToView());
            }
        }

        /// <summary>
        /// Executes the exit full-screen command on th e <see cref="FullScreenEnabledWebView"/> if available.
        /// The command is passed no parameters.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void OnExitFullscreenRequested(object sender, EventArgs eventArgs)
        {
            if (webView == null)
            {
                return;
            }

            if (webView.ExitFullScreenCommand != null && webView.ExitFullScreenCommand.CanExecute(null))
            {
                webView.ExitFullScreenCommand.Execute(null);
            }
        }
    }
}
