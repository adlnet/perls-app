using System;
using System.Globalization;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Extensions;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Episode view model.
    /// </summary>
    public class EpisodeViewModel : CardViewModel
    {
        readonly IAsyncCommand<IItem> downloadContentCommand;
        readonly IEpisode episode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeViewModel"/> class.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public EpisodeViewModel(IEpisode episode, IAsyncCommand<IItem> downloadContentCommand) : base(episode)
        {
            this.downloadContentCommand = downloadContentCommand ?? throw new ArgumentNullException(nameof(downloadContentCommand));
            this.episode = episode ?? throw new ArgumentNullException(nameof(episode));
        }

        /// <summary>
        /// Gets the model item.
        /// </summary>
        /// <value>The model item.</value>
        public IItem ModelItem => Model;

        /// <summary>
        /// Gets the episode description.
        /// </summary>
        /// <value>The description.</value>
        public string Description => episode.Description;

        /// <summary>
        /// Gets the episode track information.
        /// </summary>
        /// <value>The track information (date and duration).</value>
        public string TrackInformation => string.Format(CultureInfo.CurrentCulture, Strings.EpisodeTrackInformation, episode.ReleaseDate.ToString("MMMM d, yyyy", CultureInfo.CurrentCulture), episode.PublishedDuration.ToDurationStringFromSeconds());

        /// <summary>
        /// Gets or sets the episode's podcast.
        /// </summary>
        /// <value>The podcast.</value>
        public IPodcast Podcast
        {
            get => episode.Podcast;
            set
            {
                episode.Podcast = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the episode is complete.
        /// </summary>
        /// <value><c>true</c> if the episode is complete; otherwise, <c>false</c>.</value>
        public bool IsComplete => CompletionState.Completed;
    }
}
