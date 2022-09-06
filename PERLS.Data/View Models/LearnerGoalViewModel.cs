using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The Learner Goal View Model.
    /// </summary>
    public class LearnerGoalViewModel : BasePageViewModel
    {
        string adjustedGoalTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerGoalViewModel"/> class.
        /// </summary>
        /// <param name="goal">The goal.</param>
        /// <param name="adjustGoalCommand">The adjust goal command.</param>
        public LearnerGoalViewModel(IGoal goal, ICommand adjustGoalCommand)
        {
            Model = goal ?? throw new ArgumentNullException(nameof(goal));
            AdjustGoalCommand = adjustGoalCommand ?? throw new ArgumentNullException(nameof(adjustGoalCommand));

            SaveGoalCommand = new Command(this.HandleSaveGoal);
            AdjustedGoalTarget = GoalString;
        }

        /// <summary>
        /// Gets the adjust goal command.
        /// </summary>
        /// <value>
        /// The adjust goal command.
        /// </value>
        public ICommand AdjustGoalCommand { get; }

        /// <summary>
        /// Gets the save goal command.
        /// </summary>
        /// <value>
        /// The save goal command.
        /// </value>
        public ICommand SaveGoalCommand { get; }

        /// <summary>
        /// Gets or sets the command for when an error occurs.
        /// </summary>
        /// <value>
        /// The command for when an error occurs.
        /// </value>
        public ICommand OnErrorCommand { get; set; }

        /// <summary>
        /// Gets or sets the command for when save completes.
        /// </summary>
        /// <value>
        /// The command for when save completes.
        /// </value>
        public ICommand OnSaveCompleteCommand { get; set; }

        /// <summary>
        /// Gets the name of the type of content associated with the goal.
        /// </summary>
        /// <value>
        /// A string representing the type of content associated with the goal.
        /// </value>
        /// <remarks>
        /// Returns the content name in plural form if the current goal is not exactly 1.
        /// </remarks>
        [NotifyWhenPropertyChanges(nameof(IGoal.GoalCount))]
        public string GoalContentType => Model.Type switch
        {
            GoalType.ViewArticlesPerWeek or GoalType.CompleteArticlesPerWeek => Model.GoalCount == 1 ? Strings.ItemSingular : Strings.ItemPlural,
            GoalType.AverageTestScore => "%",
            GoalType.CompleteCoursesPerMonth => Model.GoalCount == 1 ? Strings.TypeCourse : Strings.TypeCoursePlural,
            _ => string.Empty,
        };

        /// <summary>
        /// Gets the goal title.
        /// </summary>
        /// <value>
        /// The goal title.
        /// </value>
        public string GoalTitle => Model.Type switch
        {
            GoalType.ViewArticlesPerWeek => Strings.GoalTitleWeeklyArticles,
            GoalType.AverageTestScore => Strings.GoalTitleTestScore,
            GoalType.CompleteArticlesPerWeek => Strings.GoalTitleCompletedArticles,
            GoalType.CompleteCoursesPerMonth => Strings.GoalTitleMonthlyCourses,
            _ => string.Empty,
        };

        /// <summary>
        /// Gets the goal's title as a question, as if being asked what the goal should be.
        /// </summary>
        /// <value>
        /// The goal's title as a question, as if being asked what the goal should be.
        /// </value>
        public string QuestionTitle => Model.Type switch
        {
            GoalType.ViewArticlesPerWeek => Strings.HowManyArticlesViewPerWeekLabel,
            GoalType.CompleteArticlesPerWeek => Strings.HowManyArticlesCompletePerWeekLabel,
            GoalType.CompleteCoursesPerMonth => Strings.HowManyCoursesCompletedPerMonthLabel,
            GoalType.AverageTestScore => Strings.TargetTestScoreQuestionLabel,
            _ => string.Empty,
        };

        /// <summary>
        /// Gets the user's current target for this goal, formatted for display.
        /// </summary>
        /// <value>
        /// The user's current target for this goal.
        /// </value>
        public string GoalString => $"{Model.GoalCount}";

        /// <summary>
        /// Gets or sets the goal as an int.
        /// </summary>
        /// <value>
        /// The goal as an int.
        /// </value>
        public int GoalCount
        {
            get => Model.GoalCount;

            protected set
            {
                Model.GoalCount = value;
                OnPropertyChanged(nameof(GoalCount));
                OnPropertyChanged(nameof(GoalString));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(GoalContentType));
            }
        }

        /// <summary>
        /// Gets or sets a new target value for the goal; not yet official until the user saves it.
        /// </summary>
        /// <value>
        /// A new target for the goal.
        /// </value>
        public string AdjustedGoalTarget
        {
            get => adjustedGoalTarget;
            set => SetField(ref adjustedGoalTarget, value);
        }

        /// <summary>
        /// Gets the user's current score for this goal.
        /// </summary>
        /// <value>
        /// The user's current score on this goal.
        /// </value>
        public int CurrentCount => Model.CurrentCount;

        /// <summary>
        /// Gets the current progress.
        /// </summary>
        /// <value>
        /// The current progress.
        /// </value>
        public double Progress => Math.Min(1, Model.CurrentCount / (double)Model.GoalCount);

        /// <summary>
        /// Gets an image to use as the progress indicator.
        /// </summary>
        /// <value>The progress indicator image.</value>
        public ImageSource ProgressIcon => ImageSource.FromResource("PERLS.Data.Resources.progress.gif");

        IGoal Model { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Forwards all exceptions to error handler command.")]
        async void HandleSaveGoal()
        {
            // Check the entered value to see if it has changed.
            if (!int.TryParse(AdjustedGoalTarget, out int newTarget) || newTarget == GoalCount)
            {
                // Nothing to do.
                OnSaveCompleteCommand?.Execute(this);
                AdjustedGoalTarget = GoalString;
                return;
            }

            GoalCount = newTarget;
            IsLoading = true;

            try
            {
                var learnerProvider = DependencyService.Get<ILearnerProvider>();
                var learner = DependencyService.Get<IAppContextService>().CurrentLearner;
                switch (Model.Type)
                {
                    case GoalType.AverageTestScore:
                        learner.LearnerGoals.AverageTestScoreGoal = GoalCount;
                        break;
                    case GoalType.CompleteArticlesPerWeek:
                        learner.LearnerGoals.ArticlesCompletedPerWeekGoal = GoalCount;
                        break;
                    case GoalType.CompleteCoursesPerMonth:
                        learner.LearnerGoals.CoursesCompletedPerMonthGoal = GoalCount;
                        break;
                    case GoalType.ViewArticlesPerWeek:
                        learner.LearnerGoals.ArticlesViewedPerWeekGoal = GoalCount;
                        break;
                    default:
                        return;
                }

                await learnerProvider.SaveCurrentLearnerGoals().ConfigureAwait(false);
                OnSaveCompleteCommand?.Execute(this);

                // This provides a bit of UI polish.
                // When the OnSaveCompleteCommand is executed (above), it will begin an animation to hide the popover.
                // When we set IsLoading to false (below), it will instantly hide the loading indicator on the popover.
                // The result is that the form for adjusting the goal reappears for a brief moment as the animation plays.
                // The solution is to add a slight delay before setting IsLoading to false.
                await Task.Delay(100).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                OnErrorCommand?.Execute(e);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
