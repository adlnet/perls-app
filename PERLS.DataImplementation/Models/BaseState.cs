using System;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A base class for <see cref="ItemState"/> and <see cref="TermState"/>.
    /// </summary>
    [Serializable]
    public abstract class BaseState
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; internal set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; internal set; }
    }
}
