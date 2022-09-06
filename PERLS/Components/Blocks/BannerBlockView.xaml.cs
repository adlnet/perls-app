using System;
using PERLS.Data.ViewModels.Blocks;
using Xamarin.Forms;

namespace PERLS.Components.Blocks
{
    /// <summary>
    /// The banner block view.
    /// </summary>
    public partial class BannerBlockView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BannerBlockView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public BannerBlockView(BannerBlockViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            BindingContext = viewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BannerBlockView"/> class.
        /// </summary>
        public BannerBlockView()
        {
            InitializeComponent();
        }
    }
}
