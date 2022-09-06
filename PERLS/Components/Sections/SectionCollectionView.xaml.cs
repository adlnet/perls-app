using System;
using PERLS.Data.ViewModels.Sections;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// A view to represent a collection of sections.
    /// </summary>
    public partial class SectionCollectionView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionCollectionView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to represent in this view.</param>
        public SectionCollectionView(SectionCollectionViewModel viewModel) : this()
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionCollectionView"/> class.
        /// </summary>
        public SectionCollectionView()
        {
            InitializeComponent();
        }
    }
}
