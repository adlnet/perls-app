using System.Globalization;
using PERLS.Data;
using PERLS.Data.Extensions;

namespace PERLS.Pages.Settings
{
    /// <summary>
    /// The Terms of Use page.
    /// </summary>
    public partial class TermsOfUsePage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermsOfUsePage"/> class.
        /// </summary>
        public TermsOfUsePage()
        {
            InitializeComponent();
            BindingContext = this;
            Title = StringsSpecific.ViewTermsLabel.ToUpper(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets the Terms of Use text string.
        /// </summary>
        /// <value>The terms of use string.</value>
        public string TermsOfUseText => StringsSpecific.TermsOfUseText.AddAppName();
    }
}
