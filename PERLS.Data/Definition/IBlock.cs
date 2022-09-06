using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Represents a single row of content on the dashboard.
    /// </summary>
    public interface IBlock : INamedNotifyPropertyChanged
    {
        /// <summary>
        /// Gets the layout to use for displaying the contents.
        /// </summary>
        /// <value>The content layout.</value>
        BlockTemplate Template { get; }

        /// <summary>
        /// Gets the base entity type to expect for the block contents.
        /// This is different than the item “type” which is returned on other calls; that maps to the entity bundle in Drupal.
        /// </summary>
        /// <value>The entities in this group.</value>
        EntityType Entity { get; }

        /// <summary>
        /// Gets the API endpoint to get the contents of the block.
        /// </summary>
        /// <value>The contents endpoint.</value>
        Uri ContentsUri { get; }

        /// <summary>
        /// Gets the contents dictionary.
        /// </summary>
        /// <value>
        /// The contents dictionary.
        /// </value>
        IBlockContents ContentsDictionary { get; }

        /// <summary>
        /// Gets the URL to view more; should be the actual URL for the web interface (the app should intercept and display the proper tab, if it exists).
        /// </summary>
        /// <value>The more endpoint.</value>
        Uri More { get; }

        /// <summary>
        /// Returns a task to get the contents defined by the contents Uri.
        /// </summary>
        /// <returns>An awaitable task to return contents for this block.</returns>
        Task<IEnumerable<IRemoteResource>> GetContentsTask();
    }
}
