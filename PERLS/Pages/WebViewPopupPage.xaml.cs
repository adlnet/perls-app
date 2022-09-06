using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// The discussion web view popup page.
    /// </summary>
    public partial class WebViewPopupPage : BasePopupPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewPopupPage"/> class.
        /// </summary>
        /// <param name="viewModel">The web view viewmodel.</param>
        public WebViewPopupPage(PopupWebViewViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
