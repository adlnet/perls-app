using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PERLS.Data.Commands;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A page of a deck of cards.
    /// </summary>
    public class CardDeckPageViewModel : ContentListPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardDeckPageViewModel"/> class.
        /// </summary>
        /// <param name="title">The title of the deck.</param>
        /// <param name="func">The function responsible for getting the task responsible for resolving to the deck content.</param>
        /// <param name="selectItemCommand">The command to invoke when a card is selected.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="emptyMessage">The message to present when there is no content.</param>
        /// <param name="emptyTitle">The title to present when there is no content.</param>
        /// <param name="emptyImage">The image to present when there is no content.</param>
        public CardDeckPageViewModel(string title, Func<Task<IEnumerable<IItem>>> func, ICommand selectItemCommand, IAsyncCommand<IItem> downloadContentCommand = null, string emptyMessage = null, string emptyTitle = null, string emptyImage = null) : base(title, new DeckViewModel(func, downloadContentCommand, emptyMessage, emptyTitle, emptyImage), selectItemCommand)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardDeckPageViewModel"/> class with a static set of items.
        /// </summary>
        /// <param name="title">The title of the deck.</param>
        /// <param name="items">The items in the deck.</param>
        /// <param name="selectItemCommand">The command to invoke when a card is selected.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="emptyMessage">The message to present when there is no content.</param>
        /// <param name="emptyTitle">The title to present when there is no content.</param>
        /// <param name="emptyImage">The image to present when there is no content.</param>
        public CardDeckPageViewModel(string title, IEnumerable<IItem> items, ICommand selectItemCommand, IAsyncCommand<IItem> downloadContentCommand = null, string emptyMessage = null, string emptyTitle = null, string emptyImage = null) : base(title, new DeckViewModel(items, downloadContentCommand, emptyMessage, emptyTitle, emptyImage), selectItemCommand)
        {
        }

        /// <summary>
        /// Gets the deck of cards on the page.
        /// </summary>
        /// <value>The deck of cards on the page.</value>
        public DeckViewModel Deck => List as DeckViewModel;

        /// <summary>
        /// Retrieves a card at the specified index.
        /// </summary>
        /// <param name="index">The index of the card.</param>
        /// <returns>The card at the specified index, or null if the card did not exist.</returns>
        public CardViewModel GetCardAt(int index)
        {
            return Deck.ElementAtOrDefault(index);
        }
    }
}
