using System;
using System.Windows.Input;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The base view model for the onboarding Create Account page.
    /// </summary>
    public partial class SetServerViewModel : BasePageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetServerViewModel" /> class.
        /// </summary>
        /// <param name="handleSetPressed">The command for setting the server.</param>
        public SetServerViewModel(ICommand handleSetPressed)
        {
            HandleSetPressed = handleSetPressed ?? throw new ArgumentNullException(nameof(handleSetPressed));

#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.Configuration == BuildConfiguration.Debug)
            {
                CurrentServer = Constants.DebugDefaultServer;
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        /// <summary>
        /// Gets or sets the current debug uri.
        /// </summary>
        /// <value>The current server debug URI.</value>
        public string CurrentServer { get; set; }

        /// <summary>
        /// Gets or sets the command to invoke when "Set" is pressed.
        /// </summary>
        /// <value>The set command.</value>
        public ICommand HandleSetPressed { get; set; }

        /// <summary>
        /// Displays an error message when an invalid entry is made.
        /// </summary>
        public void BadUriError()
        {
            Error = new SetServerViewModelException(Strings.EnterURLMessage);
        }
    }
}
