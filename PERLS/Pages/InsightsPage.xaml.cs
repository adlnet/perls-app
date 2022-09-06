using System;
using PERLS.Data;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// The user's goals and stats page.
    /// </summary>
    public partial class InsightsPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsightsPage"/> class.
        /// </summary>
        public InsightsPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<LearnerStatsViewModel>(
                this,
                Constants.DisplayReminderAlert,
                (sender) =>
                {
                    DisplayAlert(Strings.RemindersDisabledTitle, Strings.RemindersDisabledMessage, Strings.Okay);
                });
        }

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => true;

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is LearnerStatsViewModel learnerStatsViewModel)
            {
                learnerStatsViewModel.Refresh();
            }
        }

        /// <inheritdoc />
        protected override void OnStartLoading()
        {
            // Nothing to do here; the empty view of the page displays a loading indicator.
        }

        /// <inheritdoc/>
        protected override void OnError(Exception exception)
        {
            // Nothing to do here; the empty view of the page will show the error message.
        }
    }
}
