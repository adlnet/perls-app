using System;
using System.Linq;
using PERLS.Data.ViewModels.Sections;
using Xamarin.Forms;

namespace PERLS.Components.Sections
{
    /// <summary>
    /// A section view containing multiple blocks, with tabs to switch between them.
    /// </summary>
    public partial class TabbedSectionView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabbedSectionView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to present.</param>
        public TabbedSectionView(TabbedSectionViewModel viewModel) : this()
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabbedSectionView"/> class.
        /// </summary>
        public TabbedSectionView()
        {
            InitializeComponent();
        }

        void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BindingContext is TabbedSectionViewModel viewModel)
            {
                viewModel.BlockButtons.FirstOrDefault().IsSelected = false;
                if (e.CurrentSelection.FirstOrDefault() is BlockButtonViewModel buttonViewModel)
                {
                    viewModel.SwitchToBlock(buttonViewModel);
                }
            }

            if (e.CurrentSelection.FirstOrDefault() is BlockButtonViewModel blockButtonViewModel)
            {
                blockButtonViewModel.IsSelected = true;
            }

            if (e.PreviousSelection.FirstOrDefault() is BlockButtonViewModel oldViewModel)
            {
                oldViewModel.IsSelected = false;
            }
        }
    }
}
