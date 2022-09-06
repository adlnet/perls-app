using System;
using System.Windows.Input;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// An authenticating web view view model for the Discussions.
    /// </summary>
    public class PopupWebViewViewModel : AuthenticatingWebViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupWebViewViewModel" /> class.
        /// </summary>
        /// <param name="linkClicked">Link clicked.</param>
        /// <param name="pageFailedToLoad">Page failed to load.</param>
        /// <param name="destinationPath">Destination path.</param>
        /// <param name="closeCommand">Command to handle closing of the view.</param>
        public PopupWebViewViewModel(ICommand linkClicked, ICommand pageFailedToLoad, string destinationPath, ICommand closeCommand) : base(linkClicked, pageFailedToLoad, destinationPath)
        {
            CloseCommand = closeCommand ?? throw new ArgumentNullException(nameof(closeCommand));
        }

        /// <summary>
        /// Gets a command that's executed when close is clicked.
        /// </summary>
        /// <value>
        /// Executed when close is clicked.
        /// </value>
        public ICommand CloseCommand { get; }
    }
}
