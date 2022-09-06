using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.ViewModels;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Podcasts view model.
    /// </summary>
    public class PodcastsViewModel : CardDeckPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastsViewModel"/> class.
        /// </summary>
        /// <param name="selectCardCommand">The command to invoke when a card is selected.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public PodcastsViewModel(ICommand selectCardCommand, IAsyncCommand<IItem> downloadContentCommand) : base(string.Empty, GetCards(), selectCardCommand, downloadContentCommand)
        {
            Title = Strings.PodcastsLabel;

            Deck.EmptyImageName = "resource://PERLS.Data.Resources.empty_podcast.svg?Assembly=PERLS.Data";
            Deck.EmptyMessageTitle = Strings.EmptyPodcastTitle;
            Deck.EmptyLabel = Strings.EmptyPodcastMessage;
        }

        /// <summary>
        /// Gets the page view model for podcasts.
        /// </summary>
        /// <value>The view model for podcasts.</value>
        public IPageViewModel Podcasts { get; }

        /// <summary>
        /// Gets the cards for the dashboard.
        /// </summary>
        /// <returns>A task retrieving the card data.</returns>
        static Func<Task<IEnumerable<IItem>>> GetCards()
        {
            var provider = DependencyService.Get<ICorpusProvider>();
            return provider.GetPodcasts;
        }
    }
}
