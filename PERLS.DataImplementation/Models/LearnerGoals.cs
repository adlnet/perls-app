using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Extensions;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The LearnerGoals implementation.
    /// </summary>
    [Serializable]
    public class LearnerGoals : ILearnerGoals
    {
        int articlesViewedPerWeekGoal;
        int articlesCompletedPerWeekGoal;
        int coursesCompletedPerMonthGoal;
        int averageTestScoreGoal;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "weekly_views", NullValueHandling = NullValueHandling.Ignore)]
        public int ArticlesViewedPerWeekGoal
        {
            get => articlesViewedPerWeekGoal;

            set
            {
                articlesViewedPerWeekGoal = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ArticlesViewedPerWeekGoal)));
            }
        }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "weekly_completions", NullValueHandling = NullValueHandling.Ignore)]
        public int ArticlesCompletedPerWeekGoal
        {
            get => articlesCompletedPerWeekGoal;

            set
            {
                articlesCompletedPerWeekGoal = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ArticlesCompletedPerWeekGoal)));
            }
        }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "monthly_course_completions", NullValueHandling = NullValueHandling.Ignore)]
        public int CoursesCompletedPerMonthGoal
        {
            get => coursesCompletedPerMonthGoal;

            set
            {
                coursesCompletedPerMonthGoal = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoursesCompletedPerMonthGoal)));
            }
        }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "weekly_test_average", NullValueHandling = NullValueHandling.Ignore)]
        public int AverageTestScoreGoal
        {
            get => averageTestScoreGoal;

            set
            {
                averageTestScoreGoal = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AverageTestScoreGoal)));
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public List<Day> ReminderDays { get; } = new List<Day>();

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "notification_time", NullValueHandling = NullValueHandling.Ignore)]
        public string ReminderTime { get; set; }

        /// <inheritdoc/>
        public IEnumerable<string> ReminderDaysServerString => ReminderDays.Select((arg) => arg.StoredDay.ToServerString());

        [JsonProperty(PropertyName = "notification_days", NullValueHandling = NullValueHandling.Ignore)]
#pragma warning disable IDE0051 // Remove unused private members
        List<string> NotificationDaysString
#pragma warning restore IDE0051 // Remove unused private members
        {
            set
            {
                value.ForEach((arg) =>
                {
                    ReminderDays.Add(new Day(arg.DayOfWeekFromServerString()));
                });
            }
        }
    }
}
