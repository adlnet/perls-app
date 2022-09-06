using System;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A prompt option.
    /// </summary>
    public interface IPromptOption
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        string Key { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        string Value { get; }
    }
}
