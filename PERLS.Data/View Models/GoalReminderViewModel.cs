using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The goal reminder view model.
    /// </summary>
    public class GoalReminderViewModel : BasePageViewModel
    {
        readonly ILearner learner;
        readonly ILearnerProvider learnerProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoalReminderViewModel"/> class.
        /// </summary>
        public GoalReminderViewModel()
        {
            DaysOfWeek = new List<Day>
            {
                new Day(DayOfWeek.Sunday),
                new Day(DayOfWeek.Monday),
                new Day(DayOfWeek.Tuesday),
                new Day(DayOfWeek.Wednesday),
                new Day(DayOfWeek.Thursday),
                new Day(DayOfWeek.Friday),
                new Day(DayOfWeek.Saturday),
            };

            Times = new List<string>
            {
                "01:00",
                "02:00",
                "03:00",
                "04:00",
                "05:00",
                "06:00",
                "07:00",
                "08:00",
                "09:00",
                "10:00",
                "11:00",
                "12:00",
            };

            Periods = new List<string>
            {
                "am",
                "pm",
            };

            learner = DependencyService.Get<IAppContextService>().CurrentLearner;
            learnerProvider = DependencyService.Get<ILearnerProvider>();
            SetReminderCommand = new Command(SetReminder);
            SelectionChangedCommand = new Command(HandleSelectionChanged);

            if (learner != null)
            {
                if (learner?.LearnerGoals?.ReminderTime is string reminderTime)
                {
                    var timeStrings = reminderTime.Split();
                    SelectedTime = timeStrings[0];
                    SelectedPeriod = timeStrings[1];
                }

                foreach (var day in DaysOfWeek)
                {
                    if (learner.LearnerGoals?.ReminderDays?.Any(arg => arg.StoredDay == day.StoredDay) == true)
                    {
                        day.IsSelected = true;
                        SelectedDaysOfWeek.Add(day);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the set reminder command.
        /// </summary>
        /// <value>
        /// The set reminder command.
        /// </value>
        public Command SetReminderCommand { get; }

        /// <summary>
        /// Gets or sets the close page command.
        /// </summary>
        /// <value>
        /// The close page command.
        /// </value>
        public Command ClosePageCommand { get; set; }

        /// <summary>
        /// Gets or sets the refresh page command.
        /// </summary>
        /// <value>
        /// The refresh page command.
        /// </value>
        public Command RefreshPageCommand { get; set; }

        /// <summary>
        /// Gets the selection changed command.
        /// </summary>
        /// <value>
        /// The selection changed command.
        /// </value>
        public Command SelectionChangedCommand { get; }

        /// <summary>
        /// Gets the selected days of the week.
        /// </summary>
        /// <value>
        /// The selected days of the week.
        /// </value>
        public IList<Day> SelectedDaysOfWeek { get; } = new List<Day>();

        /// <summary>
        /// Gets or sets the selected time.
        /// </summary>
        /// <value>
        /// The selected time.
        /// </value>
        public string SelectedTime { get; set; } = "08:00";

        /// <summary>
        /// Gets or sets the selected period.
        /// </summary>
        /// <value>
        /// The selected period.
        /// </value>
        public string SelectedPeriod { get; set; } = "am";

        /// <summary>
        /// Gets the list of available days of the week.
        /// </summary>
        /// <value>
        /// The list of available days of the week.
        /// </value>
        public IList<Day> DaysOfWeek { get; }

        /// <summary>
        /// Gets the list of available reminder times.
        /// </summary>
        /// <value>
        /// The list of available reminder times.
        /// </value>
        public List<string> Times { get; }

        /// <summary>
        /// Gets the list of available periods (am and pm).
        /// </summary>
        /// <value>
        /// The list of available periods.
        /// </value>
        public List<string> Periods { get; }

        /// <summary>
        /// Gets an image to use as the progress indicator.
        /// </summary>
        /// <value>The progress indicator image.</value>
        public ImageSource ProgressIcon => ImageSource.FromResource("PERLS.Data.Resources.progress.gif");

        /// <summary>
        /// Runs the SetReminderAsync function.
        /// </summary>
        public void SetReminder()
        {
            IsLoading = true;
            Task.Run(SetReminderAsync);
        }

        /// <summary>
        /// Handles updating the selected days of week when the selection chnages.
        /// </summary>
        /// <param name="param">The collection view to be passed in.</param>
        public void HandleSelectionChanged(object param)
        {
            if (param != null && param is CollectionView collection)
            {
                if (collection.SelectedItem is not Day selDay)
                {
                    return;
                }

                selDay.IsSelected = !selDay.IsSelected;
                OnPropertyChanged(nameof(selDay.IsSelected));
                OnPropertyChanged(nameof(selDay.CellBackgroundColor));

                if (!selDay.IsSelected && SelectedDaysOfWeek.Contains(selDay))
                {
                    SelectedDaysOfWeek.Remove(selDay);
                }
                else if (selDay.IsSelected && !SelectedDaysOfWeek.Contains(selDay))
                {
                    SelectedDaysOfWeek.Add(selDay);
                }

                OnPropertyChanged(nameof(SelectedDaysOfWeek));

                collection.SelectedItem = null;
            }
        }

        /// <summary>
        /// Async method to await saving the user's goals settings.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task SetReminderAsync()
        {
            learner.LearnerGoals.ReminderTime = SelectedTime + " " + SelectedPeriod;

            learner.LearnerGoals.ReminderDays.Clear();

            foreach (Day day in SelectedDaysOfWeek)
            {
                learner.LearnerGoals.ReminderDays.Add(day);
            }

            await learnerProvider.SaveCurrentLearnerGoals().OnSuccess((task) =>
            {
                IsLoading = false;
                RefreshPageCommand?.Execute(this);
            }).OnFailure(async (task) =>
            {
                IsLoading = false;
                if (!await DependencyService.Get<INetworkConnectionService>().IsReachable().ConfigureAwait(false))
                {
                    MessagingCenter.Send(this, "ReminderSaveFailed", true);
                }
                else
                {
                    MessagingCenter.Send(this, "ReminderSaveFailed", false);
                }
            }).ConfigureAwait(false);
        }
    }
}
