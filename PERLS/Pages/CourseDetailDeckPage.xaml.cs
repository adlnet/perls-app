using System;
using System.Collections.Generic;
using System.Linq;
using Float.Core.Extensions;
using Float.Core.ViewModels;
using PERLS.Components.Cards;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PERLS.Pages
{
    /// <summary>
    /// The course detail deck page.
    /// </summary>
    public partial class CourseDetailDeckPage : BasePage
    {
        bool isPageVisible;

        /// <summary>
        /// The current index range of cards that are visible in the collection view.
        /// </summary>
        IEnumerable<int> currentVisibleRange;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseDetailDeckPage"/> class.
        /// </summary>
        /// <param name="courseDeckPageViewModel">The view model.</param>
        public CourseDetailDeckPage(CourseDeckPageViewModel courseDeckPageViewModel)
        {
            BindingContext = courseDeckPageViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public View Header { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current course has a description.
        /// </summary>
        /// <value><c>true</c> if the course has a description.</value>
        protected bool HasDescription => BindingContext is CourseDeckPageViewModel courseDeckPageViewModel && !string.IsNullOrWhiteSpace(courseDeckPageViewModel.Header?.Description);

        /// <inheritdoc/>
        public override void OnAppeared()
        {
            base.OnAppeared();

            ScrollToNextIncompleteItem();
        }

        /// <inheritdoc/>
        public override void OnFinishedRotation()
        {
            base.OnFinishedRotation();

            ScrollToNextIncompleteItem();
        }

        /// <inheritdoc/>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            UpdateHeader();
        }

        /// <inheritdoc/>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            isPageVisible = true;
            NotifyCardsGainedVisibility(currentVisibleRange);
            Collection.SelectedItem = null;

            if (BindingContext is CourseDeckPageViewModel)
            {
                if (CurrentShell != null)
                {
                    var label = new Label
                    {
                        Style = (Style)Application.Current.Resources["TextStyle"],
                        Text = Title,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        MaxLines = 2,
                        LineBreakMode = LineBreakMode.TailTruncation,
                    };
                    label.FontSize = Device.GetNamedSize(NamedSize.Small, label);

                    Shell.SetTitleView(this, label);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            isPageVisible = false;
            NotifyCardsLostVisibility(currentVisibleRange);

            UpdateHeader();
        }

        /// <inheritdoc/>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (Header != null)
            {
                Header.Resources["ItemWidth"] = Collection.Resources["ItemWidth"];
            }
        }

        ViewModel<IItem> FirstIncompleteItem()
        {
            return (BindingContext as CourseDeckPageViewModel)?.List.OfType<CardViewModel>().FirstOrDefault(arg => !arg.CompletionState.Completed);
        }

        bool HasCompleteItem()
        {
            return (BindingContext as CourseDeckPageViewModel)?.List.OfType<CardViewModel>().Any(arg => arg.CompletionState.Completed) == true;
        }

        void ScrollToNextIncompleteItem()
        {
            if (BindingContext is CourseDeckPageViewModel courseDeckPageViewModel)
            {
                courseDeckPageViewModel.Refresh();

                if (FirstIncompleteItem() is ViewModel<IItem> firstIncompleteItem && HasCompleteItem())
                {
                    var index = courseDeckPageViewModel.List.IndexOf(firstIncompleteItem) + (HasDescription ? 1 : 0);

                    Device.InvokeOnMainThreadAsync(() =>
                    {
                        Collection.ScrollTo(index, position: ScrollToPosition.Start, animate: true);
                    });
                }
            }
        }

        /// <summary>
        /// Respond to a tap gesture on a card.
        /// </summary>
        /// <remarks>
        /// In my ideal world, this wouldn't be necessary. However, it seems
        /// that a scrollview within a card intercepts all tap events so that the
        /// collection view cannot respond to the user's selection.
        /// I'm not currently aware of a better way to do this.
        /// This is (as of writing this) specificlly done for ObjectCard.
        /// </remarks>
        /// <param name="sender">The visual element that was tapped.</param>
        /// <param name="e">The event args.</param>
        void HandleCardTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement element && element.BindingContext is ISelectable item)
            {
                Collection.SelectedItem = item;

                // We want to early return if this is a document since documents aren't selectable the same way.
                if (item is TeaserViewModel teaserViewModel && teaserViewModel.ModelItem is IDocument)
                {
                    Collection.SelectedItem = null;
                }
            }
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

        void UpdateHeader()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (BindingContext is CourseDeckPageViewModel courseDeckPageViewModel && HasDescription)
                {
                    Header = new CourseDescriptionCard()
                    {
                        BindingContext = courseDeckPageViewModel.Header,
                    };

                    Header.Resources["ItemWidth"] = Collection?.Resources["ItemWidth"] ?? 0.0;

                    OnPropertyChanged(nameof(Header));
                }
                else
                {
                    Header = null;
                    OnPropertyChanged(nameof(Header));
                }
            });
        }
    }
}
