using System;
using MediaManager.Library;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class Episode : Node, IEpisode
    {
        /// <inheritdoc />
        public event MetadataUpdatedEventHandler MetadataUpdated;

        /// <summary>
        /// Gets the episode's media location, as provided by the API.
        /// </summary>
        /// <remarks>It is known that only the Url gets populated on the file object--it's not an issue.</remarks>
        /// <value>The file.</value>
        [JsonProperty("file")]
        public File PublishedMediaLocation { get; internal set; }

        /// <inheritdoc />
        IFile IEpisode.PublishedMediaLocation => PublishedMediaLocation;

        /// <summary>
        /// Gets the duration (seconds) of an episode, as provided by the API.
        /// </summary>
        /// <value>The duration of an episode in seconds..</value>
        [JsonProperty("duration")]
        public int PublishedDuration { get; internal set; }

        /// <summary>
        /// Gets the release date of an episode.
        /// </summary>
        /// <value>The release date.</value>
        [JsonProperty("release_date")]
        public DateTime ReleaseDate { get; internal set; }

        /// <inheritdoc />
        public IPodcast Podcast { get; set; }

        /// <inheritdoc />
        public string Album
        {
            get => Podcast?.Name ?? string.Empty;
            set { }
        }

        /// <inheritdoc />
        public string DisplayTitle
        {
            get => Name;
            set { }
        }

        /// <inheritdoc />
        public DateTime Date
        {
            get => ReleaseDate;
            set { }
        }

        /// <inheritdoc />
        public string DisplayDescription
        {
            get => Description;
            set { }
        }

        /// <inheritdoc />
        public TimeSpan Duration
        {
            get => TimeSpan.FromSeconds(PublishedDuration);
            set { }
        }

        /// <inheritdoc />
#pragma warning disable CA1056 // Uri properties should not be strings
        public string MediaUri
        {
            get => PublishedMediaLocation.Url.AbsoluteUri;
            set { }
        }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <inheritdoc />
        public bool IsMetadataExtracted { get; set; }

        /// <inheritdoc />
        public string Advertisement { get; set; }

        /// <inheritdoc />
        public string AlbumArtist { get; set; }

        /// <inheritdoc />
        public object AlbumImage { get; set; }

        /// <inheritdoc />
#pragma warning disable CA1056 // Uri properties should not be strings
        public string AlbumImageUri { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <inheritdoc />
        public string Artist { get; set; }

        /// <inheritdoc />
        object IMediaItem.Image { get; set; }

        /// <inheritdoc />
#pragma warning disable CA1056 // Uri properties should not be strings
        public string ImageUri { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <inheritdoc />
        public string Author { get; set; }

        /// <inheritdoc />
        public string Compilation { get; set; }

        /// <inheritdoc />
        public string Composer { get; set; }

        /// <inheritdoc />
        public int DiscNumber { get; set; }

        /// <inheritdoc />
        public object DisplayImage { get; set; }

        /// <inheritdoc />
#pragma warning disable CA1056 // Uri properties should not be strings
        public string DisplayImageUri { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <inheritdoc />
        public string DisplaySubtitle { get; set; }

        /// <inheritdoc />
        public DownloadStatus DownloadStatus { get; set; }

        /// <inheritdoc />
        public object Extras { get; set; }

        /// <inheritdoc />
        public string Genre { get; set; }

        /// <inheritdoc />
        public int NumTracks { get; set; }

        /// <inheritdoc />
        public object Rating { get; set; }

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
        public int TrackNumber { get; set; }

        /// <inheritdoc />
        public object UserRating { get; set; }

        /// <inheritdoc />
        public string Writer { get; set; }

        /// <inheritdoc />
        public int Year { get; set; }

        /// <inheritdoc />
        public string FileExtension { get; set; }

        /// <inheritdoc />
        public string FileName { get; set; }

        /// <inheritdoc />
        public MediaType MediaType { get; set; }

        /// <inheritdoc />
        public MediaLocation MediaLocation { get; set; }

        /// <inheritdoc />
        public bool IsLive { get; set; }

        /// <inheritdoc />
        string IContentItem.Id
        {
            get => Id.ToString();
            set { }
        }
    }
}
