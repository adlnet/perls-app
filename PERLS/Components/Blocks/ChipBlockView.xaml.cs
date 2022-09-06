using System;
using PERLS.Data.ViewModels.Blocks;
using Xamarin.Forms;

namespace PERLS.Components.Blocks
{
    /// <summary>
    /// A block view containing chips.
    /// </summary>
    public partial class ChipBlockView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChipBlockView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to present.</param>
        public ChipBlockView(ChipBlockViewModel viewModel) : this()
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChipBlockView"/> class.
        /// </summary>
        public ChipBlockView()
        {
            InitializeComponent();
        }
    }
}
