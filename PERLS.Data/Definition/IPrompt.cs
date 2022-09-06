using System.Collections.Generic;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A prompt.
    /// </summary>
    public interface IPrompt : IRemoteResource
    {
        /// <summary>
        /// Gets the question.
        /// </summary>
        /// <value>
        /// The question text.
        /// </value>
        string Question { get; }

        /// <summary>
        /// Gets the prompt options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        IEnumerable<IPromptOption> Options { get; }
    }
}
