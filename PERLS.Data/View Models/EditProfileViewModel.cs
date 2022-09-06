using System.Globalization;
using System.Windows.Input;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Edit profile view model.
    /// </summary>
    public class EditProfileViewModel : AuthenticatingWebViewViewModel
    {
        private readonly ICommand refreshProfileCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditProfileViewModel"/> class.
        /// </summary>
        /// <param name="currentLearner">Current learner.</param>
        /// <param name="linkClicked">Link clicked.</param>
        /// <param name="failedToLoad">Failed to load.</param>
        /// <param name="refreshProfileCommand">Refresh profile command.</param>
        public EditProfileViewModel(ILearner currentLearner, ICommand linkClicked, ICommand failedToLoad, ICommand refreshProfileCommand) : base(linkClicked, failedToLoad, currentLearner?.EditPath)
        {
            Title = currentLearner.Name;
            this.refreshProfileCommand = refreshProfileCommand;
        }

        /// <summary>
        /// Calling a command passed to execute when page disappearing.
        /// </summary>
        public void RefreshProfile()
        {
            if (refreshProfileCommand != null && refreshProfileCommand.CanExecute(null))
            {
                refreshProfileCommand.Execute(null);
            }
        }
    }
}
