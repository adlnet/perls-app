using System;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class LearnerStats : ILearnerStats
    {
        /// <inheritdoc />
        [JsonProperty("bookmarked_count_total")]
        public int BookmarkTotalCount { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("completed_count_total")]
        public int CompletedTotalCount { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("seen_count_total")]
        public int SeenTotalCount { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("completed_count_week")]
        public int CompletedWeekCount { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("seen_count_lo_week")]
        public int SeenWeekCount { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("completed_count_course_month")]
        public int CompletedCourseMonthCount { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("completed_count_course_total")]
        public int CompletedCourseTotalCount { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("result_avg_test_week")]
        public int TestWeekAverage { get; internal set; }
    }
}
