using System.Globalization;
using System.Windows.Input;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Suggestions view model.
    /// </summary>
    public class LegalInfoViewModel : AuthenticatingWebViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LegalInfoViewModel"/> class.
        /// </summary>
        /// <param name="linkClicked">Link clicked.</param>
        /// <param name="failedToLoad">Failed to load.</param>
        public LegalInfoViewModel(ICommand linkClicked, ICommand failedToLoad) : base(linkClicked, failedToLoad, Constants.LegalInfoPath)
        {
            Title = StringsSpecific.ViewTermsLabel.ToUpper(CultureInfo.CurrentCulture);
        }

        /// <inheritdoc />
        protected override bool RequiresAuthentication => false;
    }
}
