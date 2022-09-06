using System;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels;
using PERLS.DataImplementation.Models;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// The Card Deck.
    /// </summary>
    public partial class CardDeck : PERLSCollectionView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardDeck"/> class.
        /// </summary>
        public CardDeck()
        {
            InitializeComponent();
            Resources["ItemWidth"] = 300.0;
        }

        /// <inheritdoc />
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Resources["ItemWidth"] = Math.Min(width * 0.75, 500);
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
                SelectedItem = item;

                // We want to early return if this is a document since documents aren't selectable the same way.
                // This also applies to web links
                if (item is TeaserViewModel teaserViewModel && (teaserViewModel.ModelItem is IDocument || teaserViewModel.ModelItem is Node))
                {
                    SelectedItem = null;
                }
            }
        }
    }
}
