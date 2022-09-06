using System;
using System.Collections.Generic;
using Float.Core.Definitions;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Represents a group of items.
    /// </summary>
    public interface IItemGroup : INamed
    {
        /// <summary>
        /// Gets the items in the group.
        /// </summary>
        /// <value>The items in the group.</value>
        IEnumerable<IItem> Items { get; }

        /// <summary>
        /// Gets the associated url of the group.
        /// </summary>
        /// <value>The associated url of the group.</value>
        Uri Url { get; }
    }
}
