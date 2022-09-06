using System.Collections.Generic;
using PERLS.Data.ParagraphLayout.Models;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A tip item.
    /// </summary>
    public interface ITip : IItem
    {
        /// <summary>
        /// Gets the content of the card.
        /// </summary>
        /// <value>The card content.</value>
        IList<Paragraph> Body { get; }
    }
}
