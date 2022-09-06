using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using PERLS.Coordinators;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.DataImplementation;
using PERLS.DataImplementation.Providers;
using PERLS.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

[assembly: ExportFont("OpenSansBold.ttf", Alias = "Open Sans-Bold")]
[assembly: ExportFont("OpenSansBoldItalic.ttf", Alias = "Open Sans-BoldItaliac")]
[assembly: ExportFont("OpenSansExtraBold.ttf", Alias = "Open Sans-ExtraBold")]
[assembly: ExportFont("OpenSansItalic.ttf", Alias = "Open Sans-Italic")]
[assembly: ExportFont("OpenSansRegular.ttf", Alias = "Open Sans")]
[assembly: ExportFont("Lato-Bold.ttf", Alias = "Lato Medium")]

namespace PERLS
{
    /// <summary>
    /// The main xamarin app.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The name of the application resumed notification.
        /// </summary>
        public const string ApplicationResumedNotificationName = nameof(ApplicationResumedNotificationName);

        /// <summary>
        /// A list of cultures that will be included by default in non-localized applications.
        /// </summary>
        static readonly CultureInfo[] DefaultCultures = new[]
        {
            CultureInfo.InvariantCulture,
            new CultureInfo("en"),
        };

        readonly ApplicationCoordinator appCoordinator;
        bool debouncing;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();

#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.Flavor != BuildFlavor.Dev)
            {
                AppCenter.Start($"{Constants.AppCenterId}", typeof(Analytics), typeof(Crashes));
            }
#pragma warning restore CS0162 // Unreachable code detected

            // retrieve all cultures in the assembly containing our strings file
            var cultures = new HashSet<CultureInfo>(GetSupportedCultures(typeof(Strings)));

            // We set that the UI is culture-independent unless localized.
            // This avoids annoying exceptions on startup if the app figures out there are
            // no English-specific resources.
            if (cultures.SetEquals(DefaultCultures))
            {
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            }

            Float.Core.L10n.TranslateExtension.RegisterResourceManager(Data.Strings.ResourceManager);
            Float.Core.L10n.TranslateExtension.RegisterResourceManager(Data.StringsSpecific.ResourceManager);

            DependencyService.Register<LearnerStateProvider>();
            DependencyService.Register<DrupalAPI>();
            DependencyService.Register<DrupalCorpusProvider>();
            DependencyService.Register<UpdateManager>();
            DependencyService.Register<LocalHttpServer>();
            DependencyService.Register<HttpServerSecurity>();

#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.OfflineAccess)
            {
                DependencyService.Register<ICorpusProvider, CachedCorpusProvider>();
            }
#pragma warning restore CS0162 // Unreachable code detected

            DependencyService.Register<CachedLearnerProvider>();
            DependencyService.Register<LandingProvider>();
            DependencyService.Register<ProxyReportingService>();
            DependencyService.Register<Services.AppContext>();
            DependencyService.Register<Services.NetworkConnectionService>();
            DependencyService.Register<Services.SecureStoreService>();
            DependencyService.Register<Services.CachedLRS>();
            DependencyService.Register<Services.AlertNotificationDependencyHandler>();
            DependencyService.Register<Services.ResourceDownloader>();
            DependencyService.Register<Services.CacheRegistry>();
            DependencyService.Register<Services.OfflineContentService>();
            DependencyService.Register<Services.DynamicThemingService>();
            DependencyService.Register<PerlsFileProcessor>();
            DependencyService.Register<PERLS.Services.NotificationNavigation>();
            DependencyService.Register<FeatureFlagService>();
            DependencyService.Register<DocumentDecider>();
            _ = DependencyService.Get<UpdateManager>().RunCurrentUpdates();
            AppConfig.UpdateStorage();

            if (AppConfig.Culture is CultureInfo culture)
            {
                UpdateCulture(culture, false);
            }

            InteractivityHelper = new InteractivityHelper(this);
            appCoordinator = new ApplicationCoordinator();
            appCoordinator.Start();
        }

        /// <summary>
        /// Gets the current interactivity helper.
        /// </summary>
        /// <value>The current interactivity helper.</value>
        public InteractivityHelper InteractivityHelper { get; }

        /// <summary>
        /// Opens the URI.
        /// </summary>
        /// <returns><c>true</c>, if URI was opened, <c>false</c> otherwise.</returns>
        /// <param name="uri">The URI.</param>
        public async Task<bool> OpenUri(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (debouncing)
            {
                return false;
            }

            _ = Task.Delay(1000).ContinueWith(_ => this.debouncing = false, TaskScheduler.Default);

            //// Add handling for custom scheme here

            var context = DependencyService.Get<IAppContextService>();

            if (IsMissingHost(uri, context.Server))
            {
                uri = new Uri($"{context.Server.Scheme}://{context.Server.Host}{uri.PathAndQuery}{uri.Fragment}");
                if (await appCoordinator.Navigate(uri))
                {
                    return true;
                }
            }

            // SPECIAL CASE! Open documents (like PDF) in document opener.
            // iOS triggers this via AuthenticatingWebViewViewModel.HandleLoadEvent.
            // Android triggers this via
            // PERLS.Droid.Renderers.WebViewRenderer.OnDownloadStart.
            var documentDecider = DependencyService.Get<IDocumentDecider>();
            var documentOpener = DependencyService.Get<IDocumentOpener>();
            if (documentDecider.IsOpenableDocument(uri))
            {
                var file = new EmbeddedDocumentFile(uri);
                await documentOpener.OpenAsync(file).ConfigureAwait(false);
                return true;
            }

            // this may happen when attempting to view content that needs to be served using file URLs
            // for now, we just bail when this happens. but eventually we need to server content via an actual server to support this
            if (uri.Scheme == "file" && Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<Services.AlertNotificationDependencyHandler>().NotifyError(Strings.UnsupportedContentTitle, Strings.UnsupportedContentMessage);
                return false;
            }

            return OpenExternalUri(uri);
        }

        /// <summary>
        /// Treats a URI as an external URI and opens it either in an in-app browser or the device browser.
        /// </summary>
        /// <remarks>
        /// Opts to use an in-app browser for http and https URLs.
        /// </remarks>
        /// <returns><c>true</c>, if URI was opened, <c>false</c> otherwise.</returns>
        /// <param name="uri">The URI.</param>
        public bool OpenExternalUri(Uri uri)
        {
            if (uri?.Scheme == "http" || uri?.Scheme == "https")
            {
                // Add color options
                var options = new BrowserLaunchOptions
                {
                    PreferredControlColor = Color.White,
                    PreferredToolbarColor = (Color)Resources["PrimaryColor"],
                };

                DependencyService.Get<IBrowserService>().OpenBrowser(uri, options);
            }
            else if (uri?.IsAbsoluteUri == true && uri?.IsFile == true)
            {
                // Android will crash if we try to share a file that doesn't exist
                // this _could_ happen if a package was prepared incorrectly and links to a missing file
                if (!File.Exists(uri.LocalPath))
                {
                    DependencyService.Get<Services.AlertNotificationDependencyHandler>().NotifyError(Strings.FileNotFoundTitle, Strings.FileNotFoundMessage);
                    return false;
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Share.RequestAsync(new ShareFileRequest(new ShareFile(uri?.AbsolutePath))).ConfigureAwait(false);
                });
            }
            else
            {
                Launcher.OpenAsync(uri);
            }

            return true;
        }

        /// <summary>
        /// Update the culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="update">A value indicating whether or not the app coordinator should be updated.</param>
        public void UpdateCulture(CultureInfo culture, bool update = true)
        {
            // We do not want to refresh our coordinator in a case in which things aren't changing.
            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName)
            {
                return;
            }

            if (update)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Thread.CurrentThread.CurrentUICulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                    appCoordinator?.Refresh();
                });
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }

        /// <inheritdoc />
        protected override void OnSleep()
        {
            appCoordinator.OnSleep();
        }

        /// <inheritdoc />
        protected override void OnResume()
        {
            MessagingCenter.Send<Application>(this, ApplicationResumedNotificationName);
            appCoordinator.OnResume();
        }

        bool IsMissingHost(Uri uri, Uri defaultServer)
        {
            // a local file URI is not missing a host
            if (uri.IsFile && File.Exists(uri.LocalPath))
            {
                return false;
            }

            // if the URI host is empty or local, we currently assume the URI should be handled by the app coordinator
            // it's possible this is not always the case in the future
            return uri.Host == defaultServer.Host || uri.Host == "localhost" || uri.Host == "127.0.0.1" || string.IsNullOrEmpty(uri.Host);
        }

        IEnumerable<CultureInfo> GetSupportedCultures(Type type)
        {
            var rm = new ResourceManager(type);
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            foreach (var culture in cultures)
            {
                ResourceSet set;

                try
                {
                    set = rm.GetResourceSet(culture, true, false);
                }
                catch (CultureNotFoundException)
                {
                    set = null;
                }

                if (set != null)
                {
                    yield return culture;
                }
            }
        }
    }
}
