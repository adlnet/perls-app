using System;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Components.Chips
{
    /// <summary>
    /// A view containing styled text in a box.
    /// </summary>
    public partial class ChipView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChipView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to present.</param>
        public ChipView(ChipViewModel viewModel) : this()
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChipView"/> class.
        /// </summary>
        public ChipView()
        {
            InitializeComponent();
        }
    }
}
