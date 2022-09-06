using System;
using PERLS.Data.ViewModels.Sections;
using Xamarin.Forms;

namespace PERLS.Components.Sections
{
    /// <summary>
    /// A view for a section with a single column of blocks.
    /// </summary>
    public partial class OneColumnSectionView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneColumnSectionView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to present.</param>
        public OneColumnSectionView(OneColumnSectionViewModel viewModel) : this()
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneColumnSectionView"/> class.
        /// </summary>
        public OneColumnSectionView()
        {
            InitializeComponent();
        }
    }
}
