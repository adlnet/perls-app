using System;
using PERLS.Data.ViewModels;
using PERLS.Pages;

namespace PERLS.Pages
{
    /// <summary>
    /// The certificate detail page.
    /// </summary>
    public partial class CertificateDetailPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateDetailPage"/> class.
        /// </summary>
        /// <param name="certificateViewModel">The view model.</param>
        public CertificateDetailPage(CertificateViewModel certificateViewModel)
        {
            BindingContext = certificateViewModel ?? throw new ArgumentNullException(nameof(certificateViewModel));
            InitializeComponent();
        }
    }
}
