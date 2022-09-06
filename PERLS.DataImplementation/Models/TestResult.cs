using System;
using Newtonsoft.Json;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// Information about the learner's attempt on a test.
    /// </summary>
    [Serializable]
    public struct TestAttempt : IEquatable<TestAttempt>
    {
        /// <summary>
        /// Gets a value indicating whether the attempt is complete.
        /// </summary>
        /// <value><c>true</c> if the attempt is complete.</value>
        [JsonProperty("complete")]
        public bool IsComplete { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the learner passed the attempt.
        /// </summary>
        /// <value><c>true</c> if the learner passed.</value>
        public bool Passed { get; internal set; }

        /// <summary>
        /// Gets the feedback on the attempt.
        /// </summary>
        /// <value>The attempt feedback.</value>
        public string Feedback { get; internal set; }

        /// <summary>
        /// Gets the maximum possible score.
        /// </summary>
        /// <value>The maximum score.</value>
        [JsonProperty("max")]
        public double MaxScore { get; internal set; }

        /// <summary>
        /// Gets the learner's score.
        /// </summary>
        /// <value>The learner's score.</value>
        [JsonProperty("raw")]
        public double Score { get; internal set; }

        /// <summary>
        /// Determine if two <see cref="TestAttempt"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if both instances are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator ==(TestAttempt left, TestAttempt right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determine if two <see cref="TestAttempt"/> are inequal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if both instances are not equivalent, <c>false</c> otherwise.</returns>
        public static bool operator !=(TestAttempt left, TestAttempt right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is TestAttempt attempt)
            {
                return Equals(attempt);
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Feedback.GetHashCode();
        }

        /// <inheritdoc />
        public bool Equals(TestAttempt other)
        {
            // Hm....
            return false;
        }
    }
}
