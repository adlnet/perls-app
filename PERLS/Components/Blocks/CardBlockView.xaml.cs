using System;
using PERLS.Data.ViewModels.Blocks;
using Xamarin.Forms;

namespace PERLS.Components.Blocks
{
    /// <summary>
    /// A block view containing cards.
    /// </summary>
    public partial class CardBlockView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardBlockView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to present.</param>
        public CardBlockView(CardBlockViewModel viewModel) : this()
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardBlockView"/> class.
        /// </summary>
        public CardBlockView()
        {
            InitializeComponent();
        }
    }
}
