using System;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// interface for empty view in collection.
    /// </summary>
    public interface IEmptyCollectionViewModel
    {
        /// <summary>
        /// Gets the label to display when the list of items is empty.
        /// </summary>
        /// <value>The empty label.</value>
        string EmptyLabel { get; }

        /// <summary>
        /// Gets the title to display when the list of items is empty.
        /// </summary>
        /// <value>The empty label.</value>
        string EmptyMessageTitle { get; }

        /// <summary>
        /// Gets the image name to display when the list of items is empty.
        /// </summary>
        /// <value>The empty label.</value>
        string EmptyImageName { get; }
    }
}
