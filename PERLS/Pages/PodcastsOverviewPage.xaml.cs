using System.Collections.Generic;
using System.Linq;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// The page of podcasts.
    /// </summary>
    public partial class PodcastsOverviewPage : BasePage
    {
        bool isPageVisible;

        /// <summary>
        /// The current index range of cards that are visible in the collection view.
        /// </summary>
        IEnumerable<int> currentVisibleRange;

        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastsOverviewPage"/> class.
        /// </summary>
        public PodcastsOverviewPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => true;

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();

            isPageVisible = true;
            NotifyCardsGainedVisibility(currentVisibleRange);
            Collection.SelectedItem = null;

            if (BindingContext is PodcastsViewModel podcastsViewModel)
            {
                podcastsViewModel.Refresh();
            }
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            isPageVisible = false;
            NotifyCardsLostVisibility(currentVisibleRange);
        }

        /// <summary>
        /// Invoked when the collection view changes scroll position.
        /// </summary>
        /// <param name="sender">The collection view.</param>
        /// <param name="e">Event arguments with information about the new scroll position.</param>
        void OnCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var startIndex = e.FirstVisibleItemIndex;

            // Depending on the size of the screen, there may be one or more cards that are currently visible.
            // The collection view gives us the index of the first visible item and the index of the last visible item.
            var rangeCount = e.LastVisibleItemIndex - e.FirstVisibleItemIndex;
            var collection = sender as CollectionView;
            var enumerable = collection?.ItemsSource as IEnumerable<object>;
            var itemCount = enumerable?.Count() ?? 0;

            // If there is more than one card visible, then the first card _may_ be on it's
            // way off the screen so it should also be ignored and considered *not* visible.
            if (rangeCount > 1)
            {
                // Ignore the first if there is more than one card visible.
                startIndex += 1;
                rangeCount -= 1;
            }

            // If we reach the end of a deck, we count the last item as visible,
            // since it will never be in range otherwise.
            if (itemCount - 1 == e.LastVisibleItemIndex)
            {
                rangeCount += 1;
            }

            if (rangeCount < 1)
            {
                return;
            }

            UpdateVisibleRange(Enumerable.Range(startIndex, rangeCount));
        }

        /// <summary>
        /// Updates the <see cref="currentVisibleRange"/> of indexes that are visible on screen.
        /// If the page is visible, then the cards will be individually notified of their new visibility state.
        /// A card is only notified of its visibility state if it has changed.
        /// </summary>
        /// <param name="visibleRange">The new visible range of indexes.</param>
        void UpdateVisibleRange(IEnumerable<int> visibleRange)
        {
            if (isPageVisible)
            {
                if (currentVisibleRange != null)
                {
                    NotifyCardsLostVisibility(currentVisibleRange.Except(visibleRange));
                    NotifyCardsGainedVisibility(visibleRange.Except(currentVisibleRange));
                }
                else
                {
                    NotifyCardsGainedVisibility(visibleRange);
                }
            }

            currentVisibleRange = visibleRange;
        }

        /// <summary>
        /// Notifies a range of cards that they are no longer visible.
        /// </summary>
        /// <param name="indexes">The range of cards to notify.</param>
        void NotifyCardsLostVisibility(IEnumerable<int> indexes)
        {
            if (indexes == null)
            {
                return;
            }

            foreach (var index in indexes)
            {
                NotifyCardLostVisibility(index);
            }
        }

        /// <summary>
        /// Notifies a single card that it has lost visibility.
        /// </summary>
        /// <param name="index">The card index to notify.</param>
        void NotifyCardLostVisibility(int index)
        {
            GetCardAt(index)?.CardDisappearedCommand?.Execute(this);
        }

        /// <summary>
        /// Notifies a range of cards that they have become visible.
        /// </summary>
        /// <param name="indexes">The range of cards to notify.</param>
        void NotifyCardsGainedVisibility(IEnumerable<int> indexes)
        {
            if (indexes == null)
            {
                return;
            }

            foreach (var index in indexes)
            {
                NotifyCardGainedVisibility(index);
            }
        }

        /// <summary>
        /// Notifies a single card that it has gained visibility.
        /// </summary>
        /// <param name="index">The card index to notify.</param>
        void NotifyCardGainedVisibility(int index)
        {
            GetCardAt(index)?.CardAppearedCommand?.Execute(this);
        }

        /// <summary>
        /// Retrieves a card at a specified index.
        /// </summary>
        /// <param name="index">The index of the card.</param>
        /// <returns>The card, or null if there was no card.</returns>
        CardViewModel GetCardAt(int index)
        {
            if (BindingContext is not CardDeckPageViewModel deck)
            {
                return null;
            }

            return deck.GetCardAt(index);
        }
    }
}
