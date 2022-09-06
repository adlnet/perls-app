using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Extensions;
using Float.Core.Net;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Extensions;
using PERLS.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// View model for learner stats.
    /// </summary>
    public class LearnerStatsViewModel : BasePageViewModel, IEmptyCollectionViewModel
    {
        readonly ILearnerProvider learnerProvider;
        ILearnerStats learnerStats;
        ILearnerGoals learnerGoals;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerStatsViewModel"/> class.
        /// </summary>
        /// <param name="learnerProvider">The learner provider to use when requesting stats.</param>
        /// <param name="adjustGoalCommand">The adjust goal command.</param>
        /// <param name="setGoalReminderCommand">The set goal reminder command.</param>
        /// <param name="gotoGoalDetailsCommand">The view goal details command.</param>
        public LearnerStatsViewModel(ILearnerProvider learnerProvider, ICommand setGoalReminderCommand, ICommand adjustGoalCommand, ICommand gotoGoalDetailsCommand)
        {
            SetGoalReminderCommand = new Command(() =>
            {
                if (DependencyService.Get<INotificationAccessService>().ArePushNotificationsAvailable())
                {
                    setGoalReminderCommand.Execute(this);
                }
                else
                {
                    MessagingCenter.Send(this, Constants.DisplayReminderAlert);
                }
            });

            AdjustGoalCommand = adjustGoalCommand ?? throw new ArgumentNullException(nameof(adjustGoalCommand));
            ViewGoalDetailsCommand = gotoGoalDetailsCommand ?? throw new ArgumentNullException(nameof(gotoGoalDetailsCommand));
            ViewCustomGoalsCommand = DependencyService.Get<INavigationCommandProvider>().GotoCustomGoalsSelected;

            this.learnerProvider = learnerProvider ?? DependencyService.Get<ILearnerProvider>();
            Refresh();

            Title = Strings.TabMyContentLabel;
        }

        /// <summary>
        /// Gets or sets the stats enumerable.
        /// </summary>
        /// <value>
        /// The stats enumerable.
        /// </value>
        public IEnumerable<LearnerStatViewModel> Stats { get; protected set; }

        /// <summary>
        /// Gets or sets the goals enumerable.
        /// </summary>
        /// <value>
        /// The goals enumerable.
        /// </value>
        public IEnumerable<LearnerGoalViewModel> Goals { get; protected set; }

        /// <summary>
        /// Gets the set goal reminder command.
        /// </summary>
        /// <value>
        /// The set goal reminder command.
        /// </value>
        public ICommand SetGoalReminderCommand { get; }

        /// <summary>
        /// Gets the adjust goal command.
        /// </summary>
        /// <value>
        /// The adjust goal command.
        /// </value>
        public ICommand AdjustGoalCommand { get; }

        /// <summary>
        /// Gets the view goal details command.
        /// </summary>
        /// <value>
        /// The view goal details command.
        /// </value>
        public ICommand ViewGoalDetailsCommand { get; }

        /// <summary>
        /// Gets the view custom goals command.
        /// </summary>
        /// <value>
        /// The view custom goals command.
        /// </value>
        public ICommand ViewCustomGoalsCommand { get; }

        /// <inheritdoc />
        public string EmptyMessageTitle => Error?.IsOfflineException() == true ? Strings.OfflineMessageTitle : Strings.DefaultErrorTitle;

        /// <inheritdoc />
        public string EmptyLabel => Error != null ? DependencyService.Get<INotificationHandler>()?.FormatException(Error) : Strings.DefaultEmptyMessage;

        /// <inheritdoc />
        public string EmptyImageName => "error";

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();

            if (IsLoading)
            {
                return;
            }

            GetLatestStats().ContinueWith(
                task =>
                {
                    ContainsCachedData = Goals.Any() && task.Exception?.InnerException?.IsOfflineException() == true;
                    Error = task.Exception;
                }, TaskScheduler.Current);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Error):
                    OnPropertyChanged(nameof(ContainsCachedData));
                    OnPropertyChanged(nameof(EmptyMessageTitle));
                    OnPropertyChanged(nameof(EmptyLabel));
                    OnPropertyChanged(nameof(EmptyImageName));
                    break;
            }
        }

        async Task GetLatestStats()
        {
            IsLoading = true;

            try
            {
                learnerStats = await learnerProvider.GetCurrentLearnerStats().ConfigureAwait(false);

                var currentLearner = await learnerProvider.GetCurrentLearner().ConfigureAwait(false);
                learnerGoals = currentLearner.LearnerGoals;
                GenerateStats();

                // Ideally, it would be easier to know if the response we got from the provider was cached.
                if (!await DependencyService.Get<INetworkConnectionService>().IsReachable().ConfigureAwait(false))
                {
                    throw new HttpConnectionException(Strings.OfflineMessageBody.AddAppName());
                }

                Error = null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        void GenerateStats()
        {
            if (learnerStats == null)
            {
                return;
            }

            Stats = new List<LearnerStatViewModel>
            {
                new LearnerStatViewModel(learnerStats.SeenTotalCount, Strings.StatLabelSeen),
                new LearnerStatViewModel(learnerStats.CompletedTotalCount, Strings.StatLabelCompleted),
                new LearnerStatViewModel(learnerStats.BookmarkTotalCount, StringsSpecific.StatLabelBookmarked),
                new LearnerStatViewModel(learnerStats.CompletedCourseTotalCount, Strings.StatLabelCompleted, ItemType.Course),
            };

            Goals = new List<LearnerGoalViewModel>
            {
                new LearnerGoalViewModel(new Goal(GoalType.ViewArticlesPerWeek, learnerStats.SeenWeekCount, learnerGoals?.ArticlesViewedPerWeekGoal ?? 0), AdjustGoalCommand),
                new LearnerGoalViewModel(new Goal(GoalType.CompleteArticlesPerWeek, learnerStats.CompletedWeekCount, learnerGoals?.ArticlesCompletedPerWeekGoal ?? 0), AdjustGoalCommand),
                new LearnerGoalViewModel(new Goal(GoalType.AverageTestScore, learnerStats.TestWeekAverage, learnerGoals?.AverageTestScoreGoal ?? 0), AdjustGoalCommand),
                new LearnerGoalViewModel(new Goal(GoalType.CompleteCoursesPerMonth, learnerStats.CompletedCourseMonthCount, learnerGoals?.CoursesCompletedPerMonthGoal ?? 0), AdjustGoalCommand),
            };

            OnPropertyChanged(nameof(Stats));
            OnPropertyChanged(nameof(Goals));
        }
    }
}
