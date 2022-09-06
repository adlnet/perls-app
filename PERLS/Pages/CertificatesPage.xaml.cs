using PERLS.Data.ViewModels;

namespace PERLS.Pages
{
    /// <summary>
    /// The certificates page.
    /// </summary>
    public partial class CertificatesPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificatesPage"/> class.
        /// </summary>
        public CertificatesPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => true;

        /// <inheritdoc/>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Collection.SelectedItem = null;

            if (BindingContext is CertificatesViewModel certificatesViewModel)
            {
                certificatesViewModel.Refresh();
            }
        }
    }
}
