using System.Collections.Generic;
using System.ComponentModel;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The learner goals.
    /// </summary>
    public interface ILearnerGoals : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the goal for articles viewed per week.
        /// </summary>
        /// <value>
        /// The goal for articles viewed per week.
        /// </value>
        int ArticlesViewedPerWeekGoal { get; set; }

        /// <summary>
        /// Gets or sets the goal for articles completed per week.
        /// </summary>
        /// <value>
        /// The goal for articles completed per week.
        /// </value>
        int ArticlesCompletedPerWeekGoal { get; set;  }

        /// <summary>
        /// Gets or sets the goal for courses completed per month.
        /// </summary>
        /// <value>
        /// The goal for courses completed per month.
        /// </value>
        int CoursesCompletedPerMonthGoal { get; set; }

        /// <summary>
        /// Gets or sets the goal for average test scores.
        /// </summary>
        /// <value>
        /// The goal for average test scores.
        /// </value>
        int AverageTestScoreGoal { get; set; }

        /// <summary>
        /// Gets the days to remind the user.
        /// </summary>
        /// <value>
        /// The days to remind the user.
        /// </value>
        List<Day> ReminderDays { get; }

        /// <summary>
        /// Gets the reminder days as a string the server can interpret.
        /// </summary>
        /// <value>
        /// The reminder days as a string the server can interpret.
        /// </value>
        IEnumerable<string> ReminderDaysServerString { get; }

        /// <summary>
        /// Gets or sets the time to remind the user.
        /// </summary>
        /// <value>
        /// The time to remind the user.
        /// </value>
        string ReminderTime { get; set; }
    }
}
