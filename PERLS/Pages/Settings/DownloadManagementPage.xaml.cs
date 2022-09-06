using PERLS.Data.ViewModels;

namespace PERLS.Pages.Settings
{
    /// <summary>
    /// A page where the user can manage their downloads.
    /// </summary>
    public partial class DownloadManagementPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadManagementPage"/> class.
        /// </summary>
        /// <param name="model">The view model for this page.</param>
        public DownloadManagementPage(DownloadManagementViewModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}
