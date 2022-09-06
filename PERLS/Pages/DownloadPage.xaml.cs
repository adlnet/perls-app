using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// A page to display download progress.
    /// </summary>
    public partial class DownloadPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model for this page.</param>
        public DownloadPage(DownloadViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}
