namespace PERLS.Data.Definition
{
    /// <summary>
    /// High-level usage stats for a learner.
    /// </summary>
    public interface ILearnerStats
    {
        /// <summary>
        /// Gets the total number of items bookmarked.
        /// </summary>
        /// <value>The total number of items bookmarked.</value>
        int BookmarkTotalCount { get; }

        /// <summary>
        /// Gets the total number of items completed.
        /// </summary>
        /// <value>The total number of completed items.</value>
        int CompletedTotalCount { get; }

        /// <summary>
        /// Gets the total number of items seen.
        /// </summary>
        /// <value>The total number of seen items.</value>
        int SeenTotalCount { get; }

        /// <summary>
        /// Gets the amount of items completed this week.
        /// </summary>
        /// <value>
        /// The amount of items completed this week.
        /// </value>
        int CompletedWeekCount { get; }

        /// <summary>
        /// Gets the amount of items seen this week.
        /// </summary>
        /// <value>
        /// The amount of items seen this week.
        /// </value>
        int SeenWeekCount { get; }

        /// <summary>
        /// Gets the amount of courses completed this month.
        /// </summary>
        /// <value>
        /// The amount of items completed this month.
        /// </value>
        int CompletedCourseMonthCount { get; }

        /// <summary>
        /// Gets the amount of courses completed total.
        /// </summary>
        /// <value>
        /// The amount of items completed.
        /// </value>
        int CompletedCourseTotalCount { get; }

        /// <summary>
        /// Gets this months test week average.
        /// </summary>
        /// <value>
        /// This months test week average.
        /// </value>
        int TestWeekAverage { get; }
    }
}
