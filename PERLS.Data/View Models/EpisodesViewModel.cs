using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model containing the episode for a specified podcast.
    /// </summary>
    public class EpisodesViewModel : EpisodeContentListPageViewModel
    {
        readonly PodcastViewModel podcastViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodesViewModel"/> class.
        /// </summary>
        /// <param name="podcastViewModel">The podcast viewmodel.</param>
        /// <param name="selectItemCommand">The command to invoke when a user selects an item.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="corpusProvider">Uses this corpus provider for requesting the term content; defaults to current provider.</param>
        public EpisodesViewModel(PodcastViewModel podcastViewModel, ICommand selectItemCommand, IAsyncCommand<IItem> downloadContentCommand, ICorpusProvider corpusProvider = null) : base(Strings.DefaultLoadingMessage, GetContentListFunc(corpusProvider, podcastViewModel), selectItemCommand, downloadContentCommand, Strings.EpisodesEmptyMessage)
        {
            this.podcastViewModel = podcastViewModel;
        }

        /// <summary>
        /// Gets the podcast image.
        /// </summary>
        /// <value>The podcast image.</value>
        public ImageSource PodcastImageSource => podcastViewModel.Image;

        /// <summary>
        /// Gets the podcast title.
        /// </summary>
        /// <value>The podcast title.</value>
        public string PodcastTitle => podcastViewModel.Name;

        /// <summary>
        /// Gets the podcast description.
        /// </summary>
        /// <value>The podcast description.</value>
        public string PodcastDescription => podcastViewModel.Description;

        /// <summary>
        /// Gets the text for the subscribe button label.
        /// </summary>
        /// <value>The button label when the subscribe label should be displayed.</value>
        public string SubscribeButtonLabel => Strings.SubscribeButtonTitle;

        /// <summary>
        /// Gets the text for the subscribe button label.
        /// </summary>
        /// <value>The button label when the unsubscribe label should be displayed.</value>
        public string UnsubscribeButtonLabel => Strings.UnsubscribeButtonTitle;

        /// <summary>
        /// Gets the state of the bookmark.
        /// </summary>
        /// <value>The state of the bookmark.</value>
        public BookmarkStateViewModel BookmarkState => podcastViewModel.BookmarkState;

        /// <summary>
        /// Gets the item tags.
        /// </summary>
        /// <value>The item tags.</value>
        [NotifyWhenPropertyChanges(nameof(IItem.Tags))]
        public IEnumerable<TagViewModel> ItemTags => podcastViewModel?.ModelItem.Tags?.Select(tag => new TagViewModel(tag)) ?? Enumerable.Empty<TagViewModel>();

        /// <inheritdoc />
        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();
            Title = podcastViewModel.Name;

            var podcast = podcastViewModel.ModelItem as IPodcast;
            List<IEpisode> episodes = new List<IEpisode>();
            foreach (EpisodeViewModel episode in Elements)
            {
                episode.Podcast = podcast;
                episodes.Add((IEpisode)episode.ModelItem);
            }

            podcast.Episodes = episodes;
        }

        static ICorpusProvider GetCorpusProvider(ICorpusProvider corpusProvider)
        {
            return corpusProvider ?? DependencyService.Get<ICorpusProvider>();
        }

        static Func<Task<IEnumerable<IItem>>> GetContentListFunc(ICorpusProvider corpusProvider, PodcastViewModel podcastViewModel)
        {
            return () =>
            {
                return GetCorpusProvider(corpusProvider).GetEpisodes(podcastViewModel.ModelItem as IPodcast);
            };
        }
    }
}
