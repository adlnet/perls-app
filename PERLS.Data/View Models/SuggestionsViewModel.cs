using System.Globalization;
using System.Windows.Input;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Suggestions view model.
    /// </summary>
    public class SuggestionsViewModel : AuthenticatingWebViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuggestionsViewModel"/> class.
        /// </summary>
        /// <param name="linkClicked">Link clicked.</param>
        /// <param name="failedToLoad">Failed to load.</param>
        public SuggestionsViewModel(ICommand linkClicked, ICommand failedToLoad) : base(linkClicked, failedToLoad, Constants.SuggestionsPath)
        {
            Title = Strings.SendFeedbackLabel.ToUpper(CultureInfo.CurrentCulture);
        }
    }
}
