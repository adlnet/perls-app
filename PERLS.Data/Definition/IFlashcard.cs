using System.Collections.Generic;
using PERLS.Data.ParagraphLayout.Models;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Flashcard.
    /// </summary>
    public interface IFlashcard : IItem
    {
        /// <summary>
        /// Gets the content of the back.
        /// </summary>
        /// <value>The content of the back.</value>
        string BackContent { get; }

        /// <summary>
        /// Gets the content of the front of the card.
        /// </summary>
        /// <value>The content of the front of the card.</value>
        IList<Paragraph> Body { get; }

        /// <summary>
        /// Gets the content of the back of the card.
        /// </summary>
        /// <value>The content of the back of the card.</value>
        IList<Paragraph> BackBody { get; }
    }
}
