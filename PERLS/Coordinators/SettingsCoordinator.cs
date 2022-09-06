using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Commands;
using Float.Core.Notifications;
using Float.Core.UX;
using Microsoft.AppCenter.Crashes;
using PERLS.Data;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ViewModels;
using PERLS.Extensions;
using PERLS.Pages;
using PERLS.Pages.Settings;
using Xamarin.Forms;
using Destination = PERLS.Data.ViewModels.SettingsOverviewViewModel.SettingsDestination;

namespace PERLS.Coordinators
{
    /// <summary>
    /// The settings coordinator.
    /// </summary>
    public class SettingsCoordinator : Coordinator
    {
        readonly ICommand selectCommand;
        readonly Command<IDownloadable> deleteContentCommand;
        readonly Command<Exception> deleteExceptionCommand;
        Page initialPage;
        INetworkConnectionService networkService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsCoordinator"/> class.
        /// </summary>
        /// <param name="selectCommand">A command to invoke when an activity is selected.</param>
        /// <param name="deleteContentCommand">A command to invoke to delete locally cached content.</param>
        /// <param name="deleteExceptionCommand">A command to invoke when deleting content throws an exception.</param>
        public SettingsCoordinator(ICommand selectCommand, Command<IDownloadable> deleteContentCommand, Command<Exception> deleteExceptionCommand) : base()
        {
            this.selectCommand = selectCommand;
            this.deleteContentCommand = deleteContentCommand;
            this.deleteExceptionCommand = deleteExceptionCommand;
        }

        /// <inheritdoc />
        public override void Start(INavigationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            networkService = DependencyService.Get<INetworkConnectionService>();

            var navigateCommand = new NavigateCommand<Destination>(HandleNavigate);
            var closeCommand = new Command(HandleSettingsClose);
            var overviewPage = new SettingsOverviewPage(new SettingsOverviewViewModel(navigateCommand, closeCommand));
            overviewPage.DisappearingCommand = new Command(HandlePageClosing);
            var wrapper = new NavigationPage(overviewPage);
            context.PresentPage(wrapper);

            initialPage = wrapper;

            var newContext = new NavigationPageNavigationContext(wrapper);
            base.Start(newContext);
        }

        /// <inheritdoc />
        protected override Page PresentInitialPage()
        {
            return initialPage;
        }

        void HandleNavigate(NavigationOption<Destination> arg)
        {
            if (arg == null)
            {
                return;
            }

            switch (arg.Destination)
            {
                case Destination.Account:
                    Task.Run(HandleGoToAccount);
                    break;
                case Destination.Acknowledgements:
                    HandleGoToAcknowledgements();
                    break;
                case Destination.Feedback:
                    Task.Run(HandleGoToFeedback);
                    break;
                case Destination.Terms:
                    Task.Run(HandleGoToTermsOfService);
                    break;
                case Destination.DownloadManagement:
                    HandleGoToDownloadManagement();
                    break;
                case Destination.Logout:
                    HandleLogout();
                    break;
                case Destination.Groups:
                    Task.Run(HandleGoToGroups);
                    break;
                case Destination.Interests:
                    Task.Run(HandleGoToInterests);
                    break;
                case Destination.LRSCache:
                    HandleGoToLRSCache();
                    break;
                case Destination.Crash:
                    Crashes.GenerateTestCrash();
                    break;
                case Destination.Support:
                    Task.Run(HandleGoToSupport);
                    break;
                case Destination.DeviceSettings:
                    DependencyService.Get<ISettingsService>().OpenAppSettings();

                    // Workaround for a Xamarin bug, issue #7733 on their github.
                    // Android will call "OnDisappearing" when the app is sent to the background,
                    // iOS will not. As a workaround, choosing system settings will now close
                    // the in-app settings.
                    HandleSettingsClose();
                    break;
            }
        }

        async Task HandleGoToFeedback()
        {
            if (!await networkService.IsReachable().ConfigureAwait(false))
            {
                DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                return;
            }

            var editPage = new ItemWebViewPage(new SuggestionsViewModel(new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad)));
            NavigationContext.PushPage(editPage);
        }

        async void HandleGoToSupport()
        {
            if (!await networkService.IsReachable().ConfigureAwait(false))
            {
                DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                return;
            }

            var page = new ItemWebViewPage(new WebViewViewModel(new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad), Constants.SupportPath.AbsoluteUri, title: Strings.ViewSupportLabel.ToUpper(CultureInfo.CurrentCulture)));
            NavigationContext.PushPage(page);
        }

        void HandleGoToAcknowledgements()
        {
            NavigationContext.ShowDetailPage(new AcknowledgementsPage());
        }

        void HandleLogout()
        {
            if (!IsFinished)
            {
                Finish(new LogoutEventArgs());
            }
        }

        void HandleSettingsClose()
        {
            if (!IsFinished)
            {
                Finish(EventArgs.Empty);
                Shell.Current.Navigation.PopModalAsync();
            }
        }

        /// <summary>
        /// Handles closing the coordinator whenever the page is being closed.
        /// This occurs when the user presses the Android back button (which pops the current page)
        /// or when the user presses the "x" button (which also pops the current page).
        /// </summary>
        async void HandlePageClosing()
        {
            await Task.Delay(500);
            if (!IsFinished && !(Shell.Current.CurrentPage is SettingsOverviewPage))
            {
                HandleSettingsClose();
            }
        }

        async void HandleGoToTermsOfService()
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.TermsOfUse)
            {
                NavigationContext.ShowDetailPage(new TermsOfUsePage());
            }
            else
            {
                if (!await networkService.IsReachable().ConfigureAwait(false))
                {
                    DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                    return;
                }

                var page = new ItemWebViewPage(new LegalInfoViewModel(new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad)));
                NavigationContext.PushPage(page);
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        async Task HandleGoToGroups()
        {
            if (!await networkService.IsReachable().ConfigureAwait(false))
            {
                DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                return;
            }

            var context = DependencyService.Get<IAppContextService>();
            var groupsPage = new ItemWebViewPage(new WebViewViewModel(new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad), new Uri(context.Server, "user/groups").AbsoluteUri, title: StringsSpecific.GroupsTitle.ToUpper(CultureInfo.CurrentCulture)));
            NavigationContext.PushPage(groupsPage);
        }

        async Task HandleGoToInterests()
        {
            if (!await networkService.IsReachable().ConfigureAwait(false))
            {
                DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                return;
            }

            var context = DependencyService.Get<IAppContextService>();
            var groupsPage = new ItemWebViewPage(new WebViewViewModel(new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad), new Uri(context.Server, "user/interests").AbsoluteUri, title: Strings.InterestsTitle.ToUpper(CultureInfo.CurrentCulture)));
            NavigationContext.PushPage(groupsPage);
        }

        async Task HandleGoToAccount()
        {
            if (!await networkService.IsReachable().ConfigureAwait(false))
            {
                DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                return;
            }

            var appInfo = DependencyService.Get<IAppContextService>();
            var editPage = new ItemWebViewPage(new EditProfileViewModel(appInfo.CurrentLearner, new Command(HandleWebLinkClicked), new Command(WebPageFailedToLoad), new Command((obj) => appInfo.RefreshLearnerProfile())));
            NavigationContext.PushPage(editPage);
        }

        void HandleGoToLRSCache()
        {
            NavigationContext.PushPage(
                new ContentPage
                {
                    Title = "Cached Statements",
                    Content = new ScrollView
                    {
                        Content = new Label
                        {
                            Style = (Style)Application.Current.Resources["TextStyle"],
                            Text = DependencyService.Get<ILRSService>().RawLocalCache(),
                        },
                    },
                });
        }

        void HandleGoToDownloadManagement()
        {
            NavigationContext.PushPage(
                new DownloadManagementPage(
                    new DownloadManagementViewModel(
                        new Command(HandleDeleteCaches),
                        new AsyncOutCommand<bool>(HandleDeleteAll, deleteExceptionCommand),
                        deleteContentCommand,
                        new SelectViewModelCommand<IDownloadable>(HandleItemSelect))));
        }

        void HandleDeleteCaches()
        {
            DependencyService.Get<ICacheRegistryService>().ClearRegisteredCaches();
        }

        Task<bool> HandleDeleteAll()
        {
            return Device.InvokeOnMainThreadAsync(async () =>
            {
                var result = await Application.Current.MainPage
                    .DisplayAlert(Strings.DeleteDownloadsTitle, Strings.DeleteDownloadsMessage, Strings.Okay, Strings.No)
                    .ConfigureAwait(false);

                if (result)
                {
                    DependencyService.Get<IDownloaderService>().RemoveAllDownloads();
                }

                return result;
            });
        }

        void HandleItemSelect(IDownloadable item)
        {
            if (item is not IDocument)
            {
                HandleSettingsClose();
            }

            selectCommand.Execute(item);
        }

        void HandleWebLinkClicked(object arg)
        {
            if (arg is not Uri uri)
            {
                return;
            }

            Application.Current.OpenUri(uri);
        }

        void WebPageFailedToLoad(object obj)
        {
            // Delay because if you pop to fast after a push it causes a crash
            Task.Delay(500).ContinueWith(
                (arg) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        NavigationContext.PopPage(true);
                    });
                }, TaskScheduler.Current);
        }

        /// <summary>
        /// Event args for logging out.
        /// </summary>
        internal class LogoutEventArgs : EventArgs
        {
        }
    }
}
