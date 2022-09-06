using System.Collections.Generic;
using System.Windows.Input;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using PERLS.Data.ParagraphLayout.Models;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Flashcard view model.
    /// </summary>
    public class FlashcardViewModel : CardViewModel
    {
        bool isFlipped;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashcardViewModel"/> class.
        /// </summary>
        /// <param name="model">Model.</param>
        public FlashcardViewModel(IItem model) : base(model)
        {
            FlipCardCommand = new Command(() =>
            {
                IsFlipped = !IsFlipped;
                ReportingService.ReportCardFlipped(Model);
            });
        }

        /// <summary>
        /// Gets the tip content.
        /// </summary>
        /// <value>The tip content.</value>
        public string Description => Model.Description.RemoveHTMLFromString();

        /// <summary>
        /// Gets the content of the back.
        /// </summary>
        /// <value>The content of the back.</value>
        public string BackContent => ((IFlashcard)Model).BackContent.RemoveHTMLFromString();

        /// <summary>
        /// Gets a command to flip the card.
        /// </summary>
        /// <value>The card flip command.</value>
        public ICommand FlipCardCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the card is flipped.
        /// </summary>
        /// <value><c>true</c> if the card is flipped, <c>false</c> otherwise.</value>
        public bool IsFlipped
        {
            get => isFlipped;
            set => SetField(ref isFlipped, value);
        }

        /// <summary>
        /// Gets the content on the front of the card.
        /// </summary>
        /// <value>The card front content.</value>
        public IList<Paragraph> Body => ((IFlashcard)Model).Body;

        /// <summary>
        /// Gets the content on the back of the card.
        /// </summary>
        /// <value>The card back content.</value>
        public IList<Paragraph> BackBody => ((IFlashcard)Model).BackBody;

        /// <inheritdoc/>
        protected override void OnCardDisappeared()
        {
            base.OnCardDisappeared();
        }
    }
}
