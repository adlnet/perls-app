using System;
using System.Collections.Generic;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A group of nodes.
    /// </summary>
    [Serializable]
    public class NodeGroup : IItemGroup
    {
        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <summary>
        /// Gets a URL where the nodes were retrieved.
        /// </summary>
        /// <value>The node URL.</value>
        public Uri Url { get; internal set; }

        /// <summary>
        /// Gets the nodes in the group.
        /// </summary>
        /// <value>The nodes.</value>
        public List<Node> Content { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<IItem> Items => Content;
    }
}
