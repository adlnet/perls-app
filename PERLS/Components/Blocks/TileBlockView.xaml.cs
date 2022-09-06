using System;
using PERLS.Data.ViewModels.Blocks;
using Xamarin.Forms;

namespace PERLS.Components.Blocks
{
    /// <summary>
    /// A block view containing tiles.
    /// </summary>
    public partial class TileBlockView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileBlockView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to present.</param>
        public TileBlockView(TileBlockViewModel viewModel) : this()
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileBlockView"/> class.
        /// </summary>
        public TileBlockView()
        {
            InitializeComponent();
        }
    }
}
