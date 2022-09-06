using System.Windows.Input;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model that represents an action.
    /// </summary>
    /// <typeparam name="T">The type of the action parameter.</typeparam>
    /// <remarks>
    /// This can be used to add a toolbar item on pages.
    /// See how the "Follow" button works on term pages.
    /// </remarks>
    public interface IActionableViewModel<T> : IActionableViewModel
    {
        /// <summary>
        /// Gets the object to provide as the provider of the action.
        /// </summary>
        /// <value>The action parameter.</value>
        T ActionParameter { get; }
    }

    /// <summary>
    /// A view model that represents an action.
    /// </summary>
    public interface IActionableViewModel
    {
        /// <summary>
        /// Gets the label for the action.
        /// </summary>
        /// <value>The action label.</value>
        string ActionLabel { get; }

        /// <summary>
        /// Gets the command to invoke when the action is performed.
        /// </summary>
        /// <value>The action command.</value>
        ICommand ActionCommand { get; }
    }
}
