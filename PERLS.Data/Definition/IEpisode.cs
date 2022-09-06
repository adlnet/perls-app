using System;
using MediaManager.Library;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A podcast episode.
    /// </summary>
    public interface IEpisode : IItem, IMediaItem
    {
        /// <summary>
        /// Gets the media location of the episode, as provided by the API.
        /// </summary>
        /// <value>The episode media's location for the player.</value>
        IFile PublishedMediaLocation { get; }

        /// <summary>
        /// Gets the duration of an episode in seconds, as provided by the API.
        /// </summary>
        /// <value>The duration of the episode.</value>
        int PublishedDuration { get; }

        /// <summary>
        /// Gets the release date of an episode.
        /// </summary>
        /// <value>The release date.</value>
        DateTime ReleaseDate { get; }

        /// <summary>
        /// Gets or sets the podcast containing the episode.
        /// </summary>
        /// <value>The podcast the episode belongs to.</value>
        IPodcast Podcast { get; set; }
    }
}
