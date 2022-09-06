using System;
using System.Globalization;
using System.Threading.Tasks;
using Float.Core.Analytics;
using Float.Core.Extensions;
using Float.Core.Notifications;
using PERLS.Components;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Services;
using PERLS.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace PERLS.Coordinators
{
    /// <summary>
    /// The top-most coordinator in the application.
    /// Responsible for handling lifecycle events (e.g. wake, sleep, authenticate, etc.)
    /// </summary>
    public class ApplicationCoordinator : Float.Core.UX.CoordinatorParent
    {
        PrimaryNavigationCoordinator primary;

        /// <summary>
        /// Gets contextual information for the application.
        /// </summary>
        /// <value>The application context.</value>
        public IAppContextService Context => DependencyService.Get<IAppContextService>();

        /// <inheritdoc />
        public override void Start()
        {
            if (!Context.IsLearnerAuthenticated)
            {
                OnLearnerUnauthenticated();
            }
            else
            {
                if (Context.CurrentLearner.PreferredLanguage is string preferredLanguage)
                {
                    AppConfig.UpdateCulture(preferredLanguage);
                    var app = App.Current as App;
                    app.UpdateCulture(AppConfig.Culture);
                }

                DependencyService.Get<IThemingService>().UpdateTheme();
                DependencyService.Get<IFeatureFlagService>().UpdateFlagStatus(DependencyService.Get<ICorpusProvider>());
                OnLearnerAuthenticated();
            }

            base.Start();
        }

        /// <summary>
        /// Invoke when the application returns to the foreground.
        /// </summary>
        public void OnResume()
        {
            UpdateLearnerProfile();
            DependencyService.Get<IFeatureFlagService>().UpdateFlagStatus(DependencyService.Get<ICorpusProvider>());
            DependencyService.Get<IDownloaderService>().ResumeBackgroundDownloads();
            DependencyService.Get<IThemingService>().UpdateTheme();
            if (Context.IsLearnerAuthenticated)
            {
                DependencyService.Get<LocalHttpServer>().Start();
            }
        }

        /// <summary>
        /// Invoke when the application is sent to the background.
        /// </summary>
        public void OnSleep()
        {
            DependencyService.Get<IDownloaderService>().PauseBackgroundDownloads();
            DependencyService.Get<LocalHttpServer>().Stop();
        }

        /// <summary>
        /// Refreshes the app coordinator. This may be useful if there needs to be a large change to the application such as Culture.
        /// </summary>
        public void Refresh()
        {
            if (primary != null)
            {
                primary.KillCoordinator(new AppRefreshEventArgs());
                primary = null;

                primary = new PrimaryNavigationCoordinator();
                StartChildCoordinator(primary);
            }
        }

        /// <summary>
        /// Navigates to a specific piece of content in the app.
        /// </summary>
        /// <param name="uri">The URI of the content.</param>
        /// <returns><c>true</c> if the uri can be handled.</returns>
        public async Task<bool> Navigate(Uri uri)
        {
            if (primary == null)
            {
                return false;
            }

            return await primary.Navigate(uri);
        }

        /// <inheritdoc />
        protected override void HandleChildFinish(object sender, EventArgs args)
        {
            base.HandleChildFinish(sender, args);

            Device.BeginInvokeOnMainThread(() =>
            {
                if (sender is OnboardingCoordinator)
                {
                    OnLearnerAuthenticated();
                }
                else if (sender is PrimaryNavigationCoordinator && !(args is AppRefreshEventArgs))
                {
                    OnLearnerUnauthenticated();
                }
            });
        }

        /// <summary>
        /// Invoked when the current user becomes authenticated.
        /// </summary>
        void OnLearnerAuthenticated()
        {
            var learnerProvider = DependencyService.Get<ILearnerProvider>();
            UpdateLearnerProfile();

            var themeProvider = DependencyService.Get<IThemingService>();
            themeProvider.ApplyTheme(Application.Current);

            primary = new PrimaryNavigationCoordinator();
            StartChildCoordinator(primary);

#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.OfflineAccess)
            {
                DependencyService.Get<IDownloaderService>().CheckForUpdates();

                // this will trigger background downloads, if needed
                _ = DependencyService.Get<ICorpusProvider>().GetRecommendations();
            }
#pragma warning restore CS0162 // Unreachable code detected

            DependencyService.Get<IFirebaseMessagingService>().UpdatePushToken().ConfigureAwait(false);

            var context = DependencyService.Get<IAppContextService>();
            DependencyService.Get<ILRSService>().UpdateEndpoint(new Uri(context.Server, "lrs"));
            DependencyService.Get<LocalHttpServer>().Start();

            Task.Run(async () =>
            {
                ILearner learner;

                try
                {
                    learner = await learnerProvider.GetCurrentLearner().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    DependencyService.Get<AnalyticsService>().TrackException(e);
                    return;
                }

                var preferred = learner.PreferredLanguage;
                AppConfig.UpdateCulture(preferred);
                var app = App.Current as App;
                app.UpdateCulture(AppConfig.Culture);

                if (learner is ILearner theLearner && theLearner.EditPath is string editPath && !string.IsNullOrWhiteSpace(editPath) && context.Server is Uri server)
                {
                    DependencyService.Get<IAnalyticsService>().SetUserId($"{new Uri(server, editPath)}");
                }
            });
        }

        /// <summary>
        /// Invoked when the current user becomes unauthenticated.
        /// </summary>
        async void OnLearnerUnauthenticated()
        {
            var loadingIndicatorView = new LoadingIndicatorView();

            AppConfig.UpdateCulture(null);

            (App.Current as App).UpdateCulture(CultureInfo.CurrentCulture);

            if (Application.Current.MainPage == null)
            {
                Application.Current.MainPage = new Page()
                {
                    BackgroundColor = App.Current.Color("PrimaryColor"),
                };
            }
            else
            {
                await PopupNavigation.Instance.PushAsync(loadingIndicatorView);
            }

            DependencyService.Get<IDownloaderService>().RemoveAllDownloads();

            // Important Local Server is after DownloadService as DS sets FileStorage.
            DependencyService.Get<LocalHttpServer>().Stop();
            await DependencyService.Get<ILearnerStateProvider>().ClearStateCache();
            DependencyService.Get<IClearCookiesService>().ClearAllCookies();
            DependencyService.Get<IOfflineContentService>().ClearCaches();
            DependencyService.Get<IThemingService>().ResetTheme(Application.Current);

            var lrsServer = DependencyService.Get<ILRSService>();
            try
            {
                await DependencyService.Get<IFirebaseMessagingService>().DeletePushToken();
                await lrsServer.PersistQueuedStatements().ConfigureAwait(false);
                await lrsServer.PersistQueuedState().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }
            finally
            {
                Context.Logout();
                DependencyService.Get<ILRSService>().Clear();
                DependencyService.Get<ICacheRegistryService>().ClearRegisteredCaches();

                try
                {
                    await PopupNavigation.Instance.RemovePageAsync(loadingIndicatorView);
                }
                catch (Exception e)
                {
                    DependencyService.Get<AnalyticsService>().TrackException(e);
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (primary != null)
                    {
                        primary.KillCoordinator(EventArgs.Empty);
                        primary = null;
                    }

                    var login = new OnboardingCoordinator();
                    StartChildCoordinator(login);
                });
            }
        }

        /// <summary>
        /// Asks the app context to update the user's profile information.
        /// If it fails, we will log the user out.
        /// </summary>
        void UpdateLearnerProfile()
        {
            if (!Context.IsLearnerAuthenticated)
            {
                return;
            }

            Context.RefreshLearnerProfile().OnFailure((e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    OnLearnerUnauthenticated();
                    DependencyService.Get<INotificationHandler>().NotifyError(Strings.SessionExpiredErrorMessage);
                });
            });
        }
    }
}
