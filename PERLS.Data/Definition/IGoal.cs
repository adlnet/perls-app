namespace PERLS.Data.Definition
{
    /// <summary>
    /// The Goal Type enum typedef.
    /// </summary>
    public enum GoalType
    {
        /// <summary>
        /// View articles per week.
        /// </summary>
        ViewArticlesPerWeek,

        /// <summary>
        /// Complete articles per week.
        /// </summary>
        CompleteArticlesPerWeek,

        /// <summary>
        /// Complete courses per week.
        /// </summary>
        CompleteCoursesPerMonth,

        /// <summary>
        /// Complete courses total.
        /// </summary>
        CompleteCoursesTotal,

        /// <summary>
        /// Average a test score.
        /// </summary>
        AverageTestScore,
    }

    /// <summary>
    /// The Goal interface.
    /// </summary>
    public interface IGoal
    {
        /// <summary>
        /// Gets the GoalType.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        GoalType Type { get; }

        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>
        /// The current count.
        /// </value>
        int CurrentCount { get; }

        /// <summary>
        /// Gets or sets the goal count.
        /// </summary>
        /// <value>
        /// The goal count.
        /// </value>
        int GoalCount { get; set; }
    }
}
