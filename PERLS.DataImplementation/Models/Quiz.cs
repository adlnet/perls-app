using System;
using System.Collections.Generic;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class Quiz : Node, IQuiz
    {
        /// <summary>
        /// Gets the question.
        /// </summary>
        /// <value>The question.</value>
        public string Question { get; internal set; }

        /// <inheritdoc />
        public new string Name => Question;

        /// <summary>
        /// Gets the hint.
        /// </summary>
        /// <value>The hint.</value>
        public string Hint { get; internal set; }

        /// <inheritdoc />
        IEnumerable<IQuizOption> IQuiz.Options => Options;

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        public List<QuizOption> Options { get; internal set; }
    }
}
