using System.Collections.Generic;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// A horizontal scrolling list of tiles.
    /// </summary>
    public partial class GroupTileGroup : CollectionView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupTileGroup"/> class.
        /// </summary>
        public GroupTileGroup()
        {
            ItemPadding = 0;
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the padding on each tile.
        /// </summary>
        /// <value>The item padding.</value>
        public double ItemPadding
        {
            get => (double)Resources["ItemPadding"];
            set => Resources["ItemPadding"] = value;
        }
    }
}
