using System;
using PERLS.Data.ViewModels.Blocks;
using Xamarin.Forms;

namespace PERLS.Templates
{
    /// <summary>
    /// A data template selector for a collection of block view models.
    /// </summary>
    public class BlockDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template for a block of cards.
        /// </summary>
        /// <value>The card block template.</value>
        public DataTemplate CardBlockTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a block of chips.
        /// </summary>
        /// <value>The chip block template.</value>
        public DataTemplate ChipBlockTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a block of tiles.
        /// </summary>
        /// <value>The tile block template.</value>
        public DataTemplate TileBlockTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a banner.
        /// </summary>
        /// <value>The banner template.</value>
        public DataTemplate BannerBlockTemplate { get; set; }

        /// <inheritdoc />
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                CardBlockViewModel _ => CardBlockTemplate,
                ChipBlockViewModel _ => ChipBlockTemplate,
                TileBlockViewModel _ => TileBlockTemplate,
                BannerBlockViewModel _ => BannerBlockTemplate,
                _ => throw new ArgumentException(),
            };
        }
    }
}
