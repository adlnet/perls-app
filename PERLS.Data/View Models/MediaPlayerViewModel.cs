using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using Float.Core.Analytics;
using Float.Core.Extensions;
using MediaManager;
using MediaManager.Library;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ExperienceAPI;
using PERLS.Data.Extensions;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Media Player view model.
    /// </summary>
    public class MediaPlayerViewModel : Float.Core.ViewModels.BaseViewModel
    {
        readonly IReportingService reportingService = DependencyService.Get<IReportingService>();
        readonly ExperienceStateProvider stateProvider = new ExperienceStateProvider();
        readonly IList<IEpisode> mediaList;
        bool scrubberIsDragging;
        TimeSpan seekStartTime;
        IEpisode currentEpisode;
        bool firstPauseSuppressed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerViewModel"/> class.
        /// </summary>
        /// <param name="mediaList">The list of media items to be enqueued when the player is started.</param>
        public MediaPlayerViewModel(IList<IEpisode> mediaList)
        {
            this.mediaList = mediaList ?? throw new ArgumentNullException(nameof(mediaList));
            Player = CrossMediaManager.Current;
            Player.StepSizeForward = TimeSpan.FromSeconds(15);
            Player.StepSizeBackward = TimeSpan.FromSeconds(15);
            PlayPauseCommand = new Command(HandlePlayPause);
            StepBackCommand = new Command(HandleStepBack);
            StepForwardCommand = new Command(HandleStepForward);
            ScrubberDragStartedCommand = new Command(HandleScrubberDragStarted);
            ScrubberDragCompletedCommand = new Command(HandleScrubberDragCompleted);
            ScrubberValueChangedCommand = new Command(HandleScrubberValueChanged);
        }

        /// <summary>
        /// xAPI reporting events.
        /// </summary>
        public enum ReportEvent
        {
            /// <summary>
            /// The player was initialized.
            /// </summary>
            Initialized,

            /// <summary>
            /// The player played.
            /// </summary>
            Played,

            /// <summary>
            /// The player paused.
            /// </summary>
            Paused,

            /// <summary>
            /// The player seeked.
            /// </summary>
            Seeked,

            /// <summary>
            /// The player completed.
            /// </summary>
            Completed,

            /// <summary>
            /// The player terminated.
            /// </summary>
            Terminated,
        }

        /// <summary>
        /// Gets a command to play/pause the track.
        /// </summary>
        /// <value>The play/pause command.</value>
        public ICommand PlayPauseCommand { get; }

        /// <summary>
        /// Gets a command to play the previous track.
        /// </summary>
        /// <value>The play previous command.</value>
        public ICommand StepBackCommand { get; }

        /// <summary>
        /// Gets a command to play the next track.
        /// </summary>
        /// <value>The play next command.</value>
        public ICommand StepForwardCommand { get; }

        /// <summary>
        /// Gets a command to start dragging the scrubber.
        /// </summary>
        /// <value>The scrubber drag start command.</value>
        public ICommand ScrubberDragStartedCommand { get; }

        /// <summary>
        /// Gets a command to complete dragging the scrubber.
        /// </summary>
        /// <value>The scrubber drag complete command.</value>
        public ICommand ScrubberDragCompletedCommand { get; }

        /// <summary>
        /// Gets a command to indicate the scrubber value has changed.
        /// </summary>
        /// <value>The scrubber value changed command.</value>
        public ICommand ScrubberValueChangedCommand { get; }

        /// <summary>
        /// Gets or sets the current bookmark state.
        /// </summary>
        /// <value>The bookmark state.</value>
        public BookmarkStateViewModel BookmarkState { get; set; }

        /// <summary>
        /// Gets the Media Manager.
        /// </summary>
        /// <value>The media manager.</value>
        public IMediaManager Player { get; }

        /// <summary>
        /// Gets the currently playing item.
        /// </summary>
        /// <value>The playing item.</value>
        public IMediaItem CurrentMediaItem => Player.Queue.Current;

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title => CurrentMediaItem?.DisplayTitle ?? string.Empty;

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description => CurrentMediaItem?.DisplayDescription ?? string.Empty;

        /// <summary>
        /// Gets the track information.
        /// </summary>
        /// <value>The track information.</value>
        public string TrackInformation => CurrentMediaItem != null ? string.Format(CultureInfo.CurrentCulture, Strings.EpisodeTrackInformation, CurrentMediaItem.Date.ToString("MMMM d, yyyy", CultureInfo.CurrentCulture), CurrentMediaItem?.Duration.ToDurationString()) : string.Empty;

        /// <summary>
        /// Gets the album name (podcast name).
        /// </summary>
        /// <value>The album (podcast) name.</value>
        public string Podcast => CurrentMediaItem?.Album ?? string.Empty;

        /// <summary>
        /// Gets the time elapsed in the track.
        /// </summary>
        /// <value>The time elapsed.</value>
        public string TimeElapsedLabel => Player.Duration.TotalHours >= 1 ? string.Format(CultureInfo.CurrentCulture, "{0:h':'mm':'ss}", Player.Position) : string.Format(CultureInfo.CurrentCulture, "{0:m':'ss}", Player.Position);

        /// <summary>
        /// Gets the time remaining in the track.
        /// </summary>
        /// <value>The time remaining.</value>
        public string TimeRemainingLabel => Player.Duration.TotalHours >= 1 ? string.Format(CultureInfo.CurrentCulture, "-{0:h':'mm':'ss}", Player.Duration - Player.Position) : string.Format(CultureInfo.CurrentCulture, "-{0:m':'ss}", Player.Duration - Player.Position);

        /// <summary>
        /// Gets the position for the scrubber.
        /// </summary>
        /// <value>The scrubber (slider) position.</value>
        public double ScrubberPosition => Player.Duration.TotalSeconds > 0 ? Player.Position.TotalSeconds / Player.Duration.TotalSeconds : 0;

        /// <summary>
        /// Gets the play/pause button image.
        /// </summary>
        /// <value>The button image.</value>
        public string PlayPauseImage => Player.IsPlaying() ? "nowplaying_pause" : "nowplaying_play";

        /// <summary>
        /// Gets the playback speed.
        /// </summary>
        /// <value>The playback speed factor.</value>
        public double PlaybackSpeed => 1.0;

        /// <summary>
        /// Subscribe to Player events.
        /// </summary>
        public void Subscribe()
        {
            Player.StateChanged += PlayerStateChanged;
            Player.PositionChanged += PlayerPositionChanged;
            Player.Queue.QueueChanged += QueueChanged;
            Player.MediaItemFinished += PlayerMediaItemFinished;
        }

        /// <summary>
        /// Unsubscribe from Player events.
        /// </summary>
        public void Unsubscribe()
        {
            Player.StateChanged -= PlayerStateChanged;
            Player.PositionChanged -= PlayerPositionChanged;
            Player.Queue.QueueChanged -= QueueChanged;
            Player.MediaItemFinished -= PlayerMediaItemFinished;
        }

        /// <summary>
        /// Start playback of the media queue.
        /// </summary>
        public async void StartPlayback()
        {
            await Player.Play(mediaList).ConfigureAwait(false);
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        public async void StopPlayback()
        {
            ReportAudioEvent(ReportEvent.Terminated);

            if (Player.State != MediaManager.Player.MediaPlayerState.Stopped)
            {
                await Player.Stop().ConfigureAwait(false);
            }
        }

        async void HandlePlayPause()
        {
            if (Player.IsPlaying())
            {
                await Player.Pause().ConfigureAwait(false);
            }
            else
            {
                await Player.Play().ConfigureAwait(false);
            }
        }

        async void HandleStepBack()
        {
            seekStartTime = Player.Position;
            await Player.StepBackward().ConfigureAwait(false);
            ReportSeekCompleted();
        }

        async void HandleStepForward()
        {
            seekStartTime = Player.Position;
            await Player.StepForward().ConfigureAwait(false);
            ReportSeekCompleted();
        }

        void HandleScrubberDragStarted()
        {
            scrubberIsDragging = true;
            seekStartTime = Player.Position;
        }

        void HandleScrubberDragCompleted()
        {
            scrubberIsDragging = false;
            ReportSeekCompleted();
        }

        void ReportSeekCompleted()
        {
            ReportAudioEvent(ReportEvent.Seeked);
        }

        async void HandleScrubberValueChanged(object param)
        {
            if (scrubberIsDragging)
            {
                if (param is ValueChangedEventArgs args)
                {
                    await Player.SeekTo(TimeSpan.FromMilliseconds(args.NewValue * CurrentMediaItem.Duration.TotalMilliseconds)).ConfigureAwait(false);
                    OnPropertyChanged(nameof(TimeElapsedLabel));
                    OnPropertyChanged(nameof(TimeRemainingLabel));
                }
            }
        }

        /*
        The MediaManger Nuget has what "sounds" like a great event to use for when there's a new track (episode) about to play: PlayerMediaItemChanged.
        Unfortunately PlayerMediaItemChanged doesn't fire before the first track plays (so doesn't fire at all if there is only one item in the queue).
        QeuedChanged (this) method receives an event that allow us to look at the queue at a point in time.  The queue contains the IMediaItem in the Current
        property which we can use to determine when an item is first in the queue, which is our trigger that the item is now Initialized.
        */
        async void QueueChanged(object sender, MediaManager.Queue.QueueChangedEventArgs e)
        {
            if (e.MediaItem is not IEpisode newEpisode)
            {
                return;
            }

            if (newEpisode != currentEpisode)
            {
                BookmarkState = new BookmarkStateViewModel(newEpisode);
                OnPropertyChanged(nameof(BookmarkState));
                ReportAudioEvent(ReportEvent.Initialized);
                currentEpisode = newEpisode;

                try
                {
                    var resume = await stateProvider.RetrievePosition(currentEpisode).ConfigureAwait(false);
                    await Player.SeekTo(TimeSpan.FromMilliseconds(resume * CurrentMediaItem.Duration.TotalMilliseconds)).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex.IsOfflineException())
                {
                }
                catch (SendStatementException ex)
                {
                    DependencyService.Get<AnalyticsService>().TrackException(ex);
                }
            }

            UpdateTrackInfo();
        }

        void PlayerMediaItemFinished(object sender, MediaManager.Media.MediaItemEventArgs e)
        {
            ReportAudioEvent(ReportEvent.Completed);
            stateProvider.DeletePosition(currentEpisode);
        }

        void PlayerStateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            switch (e.State)
            {
                case MediaManager.Player.MediaPlayerState.Playing:
                    ReportAudioEvent(ReportEvent.Played);
                    break;
                case MediaManager.Player.MediaPlayerState.Paused:
                    ReportAudioEvent(ReportEvent.Paused);
                    break;
            }

            OnPropertyChanged(nameof(PlayPauseImage));
            UpdateScrubber();
        }

        void PlayerPositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs e)
        {
            if (!scrubberIsDragging)
            {
                UpdateScrubber();
                SavePlaybackPosition();
            }
        }

        void SavePlaybackPosition()
        {
            if (Player.State == MediaManager.Player.MediaPlayerState.Playing)
            {
                stateProvider.SavePosition(currentEpisode, ScrubberPosition);
            }
        }

        void UpdateTrackInfo()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Podcast));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(TrackInformation));
        }

        void UpdateScrubber()
        {
            OnPropertyChanged(nameof(TimeElapsedLabel));
            OnPropertyChanged(nameof(TimeRemainingLabel));
            OnPropertyChanged(nameof(ScrubberPosition));
        }

        /*
        The MediaManager Nuget reports events from the perspective of the player, possibly containing more than one item in its queue.
        The Audio xAPI spec looks at the events from the perspective of the player being a song (episode).

        Playing a queue of three episodes will result in the following order of events, if the user does not interact with the controls:
        INITIALIZED
        PAUSED
        PLAYED
        COMPLETED
        INITIALIZED
        COMPLETED
        INITIALIZED
        COMPLETED
        PAUSED
        TERMINATED (when the user exits NowPlaying)

        This is an annotated version of the events, as we'd like them to be sent:
        INITIALIZED
        --PAUSED - Ideally suppress this, low priority.
        PLAYED
        COMPLETED
        --PAUSED (manufacture)
        INITIALIZED
        --PLAYED (manufacture)
        COMPLETED
        --PAUSED (manufacture)
        INITIALIZED
        --PLAYED (manufacture)
        COMPLETED
        PAUSED
        TERMINATED (when the user exits NowPlaying)

        The logic in this method exists to annotate the events as they are received to achieve the annotated version.
         */
        void ReportAudioEvent(ReportEvent reportEvent)
        {
            switch (reportEvent)
            {
                // When Initialized fires, the track has not gone to the player yet.  CurrentMediaItem.Duration is the published length for the track, from the API.
                case ReportEvent.Initialized:
                    reportingService.ReportAudioInitialized(currentEpisode, CurrentMediaItem.Duration, PlaybackSpeed);

                    // Manufacture a paused event.
                    if (mediaList.IndexOf(currentEpisode) > 0)
                    {
                        reportingService.ReportAudioPlayed(currentEpisode, CurrentMediaItem.Duration);
                    }

                    break;
                case ReportEvent.Played:
                    reportingService.ReportAudioPlayed(currentEpisode, Player.Duration);
                    break;
                case ReportEvent.Paused:
                    if (!firstPauseSuppressed)
                    {
                        firstPauseSuppressed = true;
                        break;
                    }

                    reportingService.ReportAudioPaused(currentEpisode, Player.Duration, ScrubberPosition, Player.Position);
                    break;
                case ReportEvent.Seeked:
                    reportingService.ReportAudioSeeked(currentEpisode, seekStartTime, Player.Position);
                    break;
                case ReportEvent.Completed:
                    reportingService.ReportAudioCompleted(currentEpisode, Player.Duration, ScrubberPosition, Player.Position);
                    DependencyService.Get<ILearnerStateProvider>().GetState(currentEpisode).MarkAsComplete();

                    // Manufacture the played event.
                    var episodeIndex = mediaList.IndexOf(currentEpisode);
                    if (episodeIndex < mediaList.Count - 1)
                    {
                        reportingService.ReportAudioPaused(currentEpisode, Player.Duration, ScrubberPosition, Player.Position);
                    }

                    break;
                case ReportEvent.Terminated:
                    if (Player.Position != TimeSpan.Zero)
                    {
                        reportingService.ReportAudioTerminated(currentEpisode, Player.Duration, Player.Duration.TotalSeconds > 0 ? Player.Position.TotalSeconds / Player.Duration.TotalSeconds : 0, Player.Position);
                    }
                    else
                    {
                        reportingService.ReportAudioTerminated(currentEpisode, CurrentMediaItem.Duration);
                    }

                    break;
            }
        }
    }
}
