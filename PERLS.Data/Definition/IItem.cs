using System.Collections.Generic;
using System.ComponentModel;
using Float.Core.Definitions;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A generic item in the corpus.
    /// </summary>
    public interface IItem : INamed, IRemoteResource, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the image associated with the item.
        /// </summary>
        /// <value>The image associated with the item.</value>
        IFile Image { get; }

        /// <summary>
        /// Gets a list of tags associated with the item.
        /// </summary>
        /// <value>A list of associated tags.</value>
        IEnumerable<ITag> Tags { get; }

        /// <summary>
        /// Gets the topic containing this item.
        /// </summary>
        /// <value>The parent topic.</value>
        ITopic Topic { get; }

        /// <summary>
        /// Gets a description of the item.
        /// </summary>
        /// <value>A description of this item.</value>
        string Description { get; }

        /// <summary>
        /// Gets a type of the item.
        /// </summary>
        /// <value>The type of this item.</value>
        string Type { get; }
    }
}
