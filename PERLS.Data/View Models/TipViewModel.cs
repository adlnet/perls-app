using System.Collections.Generic;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using PERLS.Data.ParagraphLayout.Models;
using TinCan;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model representing a tip in a card.
    /// </summary>
    public class TipViewModel : CardViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TipViewModel"/> class.
        /// </summary>
        /// <param name="model">The tip model.</param>
        public TipViewModel(IItem model) : base(model)
        {
        }

        /// <summary>
        /// Gets the tip content.
        /// </summary>
        /// <value>The tip content.</value>
        public string Description => Model.Description.RemoveHTMLFromString();

        /// <summary>
        /// Gets the tip content.
        /// </summary>
        /// <value>The body.</value>
        public IList<Paragraph> Body => ((ITip)Model).Body;

        /// <inheritdoc/>
        protected override void OnCardDisappeared()
        {
            base.OnCardDisappeared();
        }
    }
}
