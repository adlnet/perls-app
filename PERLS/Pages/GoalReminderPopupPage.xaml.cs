using System;
using Float.Core.Notifications;
using PERLS.Data;
using PERLS.Data.ViewModels;
using PERLS.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// The Goal Reminder Popup Page.
    /// </summary>
    public partial class GoalReminderPopupPage : PopupPage
    {
        LearnerStatsViewModel learnerStatsViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoalReminderPopupPage"/> class.
        /// </summary>
        /// <param name="goalReminderViewModel">The goal reminder view model.</param>
        /// <param name="learnerStatsViewModel">The learner stats view model.</param>
        public GoalReminderPopupPage(GoalReminderViewModel goalReminderViewModel, LearnerStatsViewModel learnerStatsViewModel)
        {
            BindingContext = goalReminderViewModel;
            this.learnerStatsViewModel = learnerStatsViewModel;
            goalReminderViewModel.RefreshPageCommand = new Command(HandleRefreshPage);
            goalReminderViewModel.ClosePageCommand = new Command(HandleClosePage);
            InitializeComponent();

            MessagingCenter.Subscribe<GoalReminderViewModel, bool>(this, "ReminderSaveFailed", (sender, isOfflineError) =>
            {
                if (isOfflineError)
                {
                    DependencyService.Get<INotificationHandler>().NotifyDeviceOffline();
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(Strings.RemindersErrorTitle, Strings.RemindersErrorMessage, Strings.Okay);
                    });
                }
            });
        }

        /// <summary>
        /// Handles closing the reminder pop up page.
        /// </summary>
        void HandleClosePage()
        {
            _ = PopupNavigation.Instance.PopAsync();
        }

        /// <summary>
        /// Handles closing the reminder pop up page and refreshing the learner stats
        /// after saving the stats to the server.
        /// </summary>
        void HandleRefreshPage()
        {
            learnerStatsViewModel.Refresh();
            _ = PopupNavigation.Instance.PopAsync();
        }
    }
}
