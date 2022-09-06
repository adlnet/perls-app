using System.Collections.Generic;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A quiz item.
    /// </summary>
    public interface IQuiz : IItem
    {
        /// <summary>
        /// Gets a list of answer options.
        /// </summary>
        /// <value>A list of answer options.</value>
        IEnumerable<IQuizOption> Options { get; }
    }
}
