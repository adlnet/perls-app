using System;
using Float.Core.Notifications;
using PERLS.Data.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// The adjusting of a goal popup page.
    /// </summary>
    public partial class GoalAdjustmentPopupPage : PopupPage
    {
        const int MinimumGoalValue = 0;
        const int MaximumGoalValue = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoalAdjustmentPopupPage"/> class.
        /// </summary>
        /// <param name="learnerGoalViewModel">The learner goal view model.</param>
        public GoalAdjustmentPopupPage(LearnerGoalViewModel learnerGoalViewModel)
        {
            learnerGoalViewModel.OnErrorCommand = new Command(HandleError);
            learnerGoalViewModel.OnSaveCompleteCommand = new Command(HandleSaveFinished);
            BindingContext = learnerGoalViewModel;
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GoalEntry.Focus();

            // Sets the text cursor to the end of the field to make it easier to edit.
            GoalEntry.CursorPosition = GoalEntry.Text.Length;
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            GoalEntry.Unfocus();
        }

        /// <summary>
        /// A method to handle Close button tap event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        void CloseButtonTappedEventHandler(object sender, EventArgs args)
        {
            PopupNavigation.Instance.RemovePageAsync(this, true);
        }

        void HandleError(object arg)
        {
            if (arg is Exception e)
            {
                DependencyService.Get<INotificationHandler>().NotifyException(e, Data.Strings.DefaultErrorTitle);
            }
        }

        void HandleSaveFinished(object arg)
        {
            PopupNavigation.Instance.RemovePageAsync(this, true);
        }

        /// <summary>
        /// Invoked when the user changes the value of their goal.
        /// </summary>
        /// <param name="sender">The input field.</param>
        /// <param name="e">The event.</param>
        /// <remarks>
        /// We want to limit the input to only be numbers and between a specific range.
        /// </remarks>
        void HandleTextChanged(Entry sender, TextChangedEventArgs e)
        {
            // Allow empty values.
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                return;
            }

            // Validate that the entered goal target is numeric
            // and within the acceptable range.
            if (!IsValidGoalTarget(e.NewTextValue))
            {
                sender.Text = e.OldTextValue;
            }
        }

        /// <summary>
        /// Checks whether a string value represents a valid goal target.
        /// A valid goal target must be numeric and between (inclusive) of the minimum and maximum goal values.
        /// </summary>
        /// <param name="value">The proposed new goal target.</param>
        /// <returns><c>true</c> if it is valid.</returns>
        bool IsValidGoalTarget(string value)
        {
            return int.TryParse(value, out int numericValue)
                && numericValue >= MinimumGoalValue
                && numericValue <= MaximumGoalValue;
        }
    }
}
