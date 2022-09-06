namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A type that provides a view model that represents an action (i.e. follow or bookmark state).
    /// </summary>
    /// <remarks>
    /// When used as the BindingContext for a BasePage, a toolbar item is added to the page.
    /// </remarks>
    public interface IActionableProvider
    {
        /// <summary>
        /// Gets the view model representing the action (label, command, etc.).
        /// </summary>
        /// <value>The view model representing the action.</value>
        IActionableViewModel Action { get; }
    }
}
