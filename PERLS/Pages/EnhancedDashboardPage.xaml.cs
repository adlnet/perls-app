using System;
using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// The page for displaying the enhanced dashboard.
    /// </summary>
    public partial class EnhancedDashboardPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnhancedDashboardPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to represent on this page.</param>
        public EnhancedDashboardPage(EnhancedDashboardViewModel viewModel) : this()
        {
            BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnhancedDashboardPage"/> class.
        /// </summary>
        public EnhancedDashboardPage() : base()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public override bool ShowLogo => true;

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => true;

        /// <summary>
        /// Perform a refresh of this page's data. Should be used sparingly.
        /// </summary>
        public void Refresh()
        {
            if (BindingContext is EnhancedDashboardViewModel viewModel)
            {
                viewModel.Refresh();
            }
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is EnhancedDashboardViewModel viewModel)
            {
                viewModel.Refresh();
            }
        }
    }
}
