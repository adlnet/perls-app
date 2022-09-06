using System.Collections.Generic;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A Test item.
    /// </summary>
    public interface ITest : IItem
    {
        /// <summary>
        /// Gets the questions (As represented by quizzes).
        /// </summary>
        /// <value>
        /// The questions (As represented by quizzes).
        /// </value>
        IEnumerable<IQuiz> Questions { get; }

        /// <summary>
        /// Gets the percent correct required to pass (assuming 0.0 is all wrong, 1.0 is all right).
        /// </summary>
        /// <value>
        /// The percent correct required to pass (assuming 0.0 is all wrong, 1.0 is all right).
        /// </value>
        double PercentRequiredToPass { get; }
    }
}
