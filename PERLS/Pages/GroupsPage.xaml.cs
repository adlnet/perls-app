using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// The following page.
    /// </summary>
    public partial class GroupsPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsPage"/> class.
        /// </summary>
        public GroupsPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => true;

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is BasePageViewModel viewModel)
            {
                viewModel.Refresh();
            }
        }
    }
}
