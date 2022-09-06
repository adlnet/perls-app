using static PERLS.Data.Definition.IGoal;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A Goal.
    /// </summary>
    public class Goal : IGoal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Goal"/> class.
        /// </summary>
        public Goal()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Goal"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="currentCount">The current count.</param>
        /// <param name="goalCount">The goal count.</param>
        public Goal(GoalType type, int currentCount, int goalCount)
        {
            Type = type;
            CurrentCount = currentCount;
            GoalCount = goalCount;
        }

        /// <inheritdoc/>
        public GoalType Type { get; protected set; }

        /// <inheritdoc/>
        public int CurrentCount { get; protected set; }

        /// <inheritdoc/>
        public int GoalCount { get; set; }
    }
}
