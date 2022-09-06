using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Float.Core.Analytics;
using Float.Core.Commands;
using Float.Core.Net;
using Float.Core.Notifications;
using Float.Core.UX;
using PERLS.Data;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ViewModels;
using PERLS.DataImplementation;
using PERLS.DataImplementation.Providers;
using PERLS.Pages;
using PERLS.Pages.Settings;
using Xamarin.Forms;

namespace PERLS.Coordinators
{
    /// <summary>
    /// The onboarding coordinator. The coordinator responsible for login and the associated acts.
    /// </summary>
    public class OnboardingCoordinator : Coordinator
    {
        Page initialPage;
        LoginViewModel loginViewModel;
        LandingGroupViewModel landingViewModel;

        /// <inheritdoc />
        public override void Start(INavigationContext context)
        {
            AppConfig.Server = null;
            loginViewModel = new LoginViewModel(new DebounceCommand(HandleLoginSelected, 1200), new Command(HandleLoginCompleted), new Command<Uri>(HandleLoginFailed), new AsyncCommand(WebPageFailedToLoad), Constants.PrefersLocalAuthentication);
            var landingService = DependencyService.Get<ILandingProvider>();
            landingViewModel = new LandingGroupViewModel(new DebounceCommand(HandleLoginSelected, 1200), new Command(HandleTermsOfUseSelected), landingService.GetLandingData(), new Command(HandleIconTapped), AppConfig.Server);
            var landingPage = new OnboardingPage(landingViewModel);
            var wrapper = new NavigationPage(landingPage);
            var newContext = new NavigationPageNavigationContext(wrapper);
            newContext.PushPage(landingPage);

            initialPage = wrapper;

            Application.Current.MainPage = initialPage;

            base.Start(newContext);
        }

        /// <inheritdoc />
        protected override Page PresentInitialPage()
        {
            return initialPage;
        }

        async void HandleIconTapped()
        {
            landingViewModel.TapCount++;

            if (landingViewModel.TapCount > 4)
            {
                var result = await Application.Current.MainPage
                    .DisplayAlert(
                    Strings.LocalLoginLabel,
                    loginViewModel.EnableLocalAuthentication ? Strings.DisableLocalLoginMessage : Strings.EnableLocalLoginMessage,
                    loginViewModel.EnableLocalAuthentication ? Strings.Disable : Strings.Enable,
                    Strings.Cancel)
                    .ConfigureAwait(false);

                landingViewModel.TapCount = 0;
                if (result)
                {
                    loginViewModel.EnableLocalAuthentication = !loginViewModel.EnableLocalAuthentication;
                }
            }
        }

        void HandleTermsOfUseSelected()
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.TermsOfUse)
            {
                NavigationContext.ShowDetailPage(new TermsOfUsePage());
            }
            else
            {
                var page = new ItemWebViewPage(new LegalInfoViewModel(new Command(HandleWebLinkClicked), new AsyncCommand(WebPageFailedToLoad)));
                NavigationContext.PushPage(page);
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        void HandleLoginSelected()
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.TeamEntry)
            {
                var setServerPage = new SetServerPage(new SetServerViewModel(new Command(HandleSetPressed)));
                NavigationContext.PushPage(setServerPage);
            }
            else
            {
                var page = new AuthenticatePage(loginViewModel);
                NavigationContext.PushPage(page);
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        void HandleSetPressed(object obj)
        {
            // ensure caches from previous logins are cleared
            DiskProviderCache.ClearCacheFolder();

            if (obj is not SetServerViewModel viewModel)
            {
                return;
            }

            if (string.IsNullOrEmpty(viewModel.CurrentServer))
            {
                viewModel.BadUriError();
                return;
            }

            var textToTest = viewModel.CurrentServer.Trim();
            var isUri = textToTest.Contains(".");
            var isWellFormedUri = Uri.TryCreate(textToTest, UriKind.Absolute, out Uri uriResult) && isUri
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.Flavor == BuildFlavor.Release)
            {
                // In release builds we don't want to allow inputing URIs only team names
                if (uriResult != null && !uriResult.Host.EndsWith(Constants.AllowedHost, StringComparison.InvariantCulture))
                {
                    viewModel.BadUriError();
                    return;
                }
            }
#pragma warning restore CS0162 // Unreachable code detected

            // URIs without a / at the end cause the webview to load forever
            if (isUri)
            {
                if (!textToTest.EndsWith("/"))
                {
                    textToTest += '/';
                }
            }
            else
            {
                textToTest = CreateTeamURI(textToTest);
                isWellFormedUri = Uri.TryCreate(textToTest, UriKind.Absolute, out Uri uriResult2)
                    && (uriResult2.Scheme == Uri.UriSchemeHttp || uriResult2.Scheme == Uri.UriSchemeHttps);
            }

            if (isWellFormedUri)
            {
                AppConfig.Server = new Uri(textToTest);
                landingViewModel.UpdateUri(AppConfig.Server);
                var networkConnectionService = DependencyService.Get<INetworkConnectionService>();
                networkConnectionService.RefreshAuthStrategy();
                var drupalAPI = DependencyService.Get<DrupalAPI>();
                drupalAPI.UpdateRequestClient();

                // update login URI
                loginViewModel.Refresh();

                // refreshing the auth strategy creates a new instance
                // so here we need to subscribe again (we had previously subscribed in the constructor)
                if (networkConnectionService.AuthStrategy is OAuth2StrategyAuthCode authCodeStrategy)
                {
                    authCodeStrategy.OnAuthCodeReceived += loginViewModel.OnAuthorizationCodeReceived;
                }

                var page = new AuthenticatePage(loginViewModel);
                NavigationContext.PushPage(page);
            }
            else
            {
                viewModel.BadUriError();
            }
        }

        string CreateTeamURI(string teamName)
        {
            var teamWithValidCharacters = teamName.Replace(' ', '-');
            teamWithValidCharacters = string.Concat(teamWithValidCharacters.Split(Path.GetInvalidFileNameChars())).ToLowerInvariant();
            return $"{Constants.DefaultServer.Scheme}://{teamWithValidCharacters}.{Constants.DefaultServer.Host}";
        }

        /// <summary>
        /// Invoked following the successful authentication of a user.
        /// </summary>
        /// <remarks>
        /// Following a successful login, the app needs to retrieve configuration data (i.e. theming)
        /// before letting the user into the app.
        /// </remarks>
        void HandleLoginCompleted()
        {
            NavigationContext.PushPage(new FinishingOnboardingPage());
            DependencyService.Get<IThemingService>().UpdateTheme().ContinueWith(task =>
            {
                // It's okay if this task failed.
                // It's possible the server doesn't support dynamic theming
                // or that some other network error occurred. Either way,
                // we'll let the user into the app and try again later.
                if (task.Exception != null)
                {
                    DependencyService.Get<AnalyticsService>().TrackException(task.Exception);
                }

                DependencyService.Get<IFeatureFlagService>().UpdateFlagStatus(DependencyService.Get<ICorpusProvider>()).ContinueWith(task2 =>
                {
                    if (task2.Exception != null)
                    {
                        DependencyService.Get<AnalyticsService>().TrackException(task.Exception);
                    }

                    DependencyService.Get<IFirebaseMessagingService>().RequestPushPermission();
                    HandleOnboardingFinished();
                });
            });
        }

        void HandleOnboardingFinished()
        {
            Finish(EventArgs.Empty);
        }

        void HandleLoginFailed(Uri uri)
        {
            if (uri != null)
            {
                Application.Current.OpenUri(uri);
            }

            NavigationContext.PopPage();
        }

        void HandleWebLinkClicked(object arg)
        {
            if (arg is not Uri uri)
            {
                return;
            }

            Application.Current.OpenUri(uri);
        }

        async Task WebPageFailedToLoad(object obj)
        {
            if (obj is HttpConnectionException e)
            {
                var handler = DependencyService.Get<INotificationHandler>();

                if (await IsReachable(Constants.DefaultReachabilityUri).ConfigureAwait(false))
                {
#pragma warning disable CS0162 // Unreachable code detected
                    if (Constants.TeamEntry)
                    {
                        // user entered an invalid team name
                        handler.NotifyError(Strings.EnterURLMessage);
                    }
                    else
                    {
                        // this may happen if we misconfigure a new app with a bad default server uri
                        var analytics = DependencyService.Get<AnalyticsService>();
                        analytics.TrackException(e);

                        handler.NotifyError(Strings.UnknownErrorMessage);
                    }
#pragma warning restore CS0162 // Unreachable code detected
                }

                // on android, the alert comes from the webview
                // you might see an offline error for invalid URIs, but not for invalid team names
                else if (Device.RuntimePlatform != Device.Android)
                {
                    // if we can't even reach the default website the user is probably offline
                    handler.NotifyException(e);
                }
            }

            // Delay because if you pop too fast after a push it causes a crash
            await Task.Delay(500);

            await Device.InvokeOnMainThreadAsync(() =>
            {
                NavigationContext.PopPage(true);
            });
        }

        async Task<bool> IsReachable(Uri uri)
        {
            var config = new ClientHandlerConfiguration
            {
                AllowCookies = false,
            };

            var handler = DependencyService.Get<INativeHttpClientHandler>().CreateHandler(config);

            try
            {
                using (var client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 10) })
                using (var request = new HttpRequestMessage(HttpMethod.Head, uri))
                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    return (int)response.StatusCode > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
