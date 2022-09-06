using System;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The implementation for the prompt option.
    /// </summary>
    [Serializable]
    public class PromptOption : IPromptOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromptOption"/> class.
        /// </summary>
        public PromptOption()
        {
        }

        /// <inheritdoc/>
        public string Key { get; internal set; }

        /// <inheritdoc/>
        public string Value { get; internal set; }
    }
}
