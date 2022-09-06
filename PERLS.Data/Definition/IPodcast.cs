using System.Collections.Generic;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A podcast.
    /// </summary>
    public interface IPodcast : IItem
    {
        /// <summary>
        /// Gets or sets the episodes in a podcast.
        /// </summary>
        /// <value>The episodes.</value>
        IEnumerable<IEpisode> Episodes { get; set; }

        /// <summary>
        /// Gets or sets the count of the episodes.
        /// </summary>
        /// <value>The count of the episodes.</value>
        int EpisodesCount { get; set; }
    }
}
