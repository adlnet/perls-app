using Xamarin.Forms;

namespace PERLS.Data
{
    /// <summary>
    /// Handles C# actions invoked by given JavaScript.
    /// </summary>
    public interface IJavaScriptActionHandler
    {
        /// <summary>
        /// Getsthe JavaScript to attach to the WebView.
        /// </summary>
        /// <value>The JavaScript to attach.</value>
        string AttachedJavaScript { get; }

        /// <summary>
        /// Gets the unique name of the action.
        /// </summary>
        /// <value>Name of the action.</value>
        string ActionName { get; }

        /// <summary>
        /// The action to perform.
        /// </summary>
        /// <param name="element">The element of the effect (most likely WebView).</param>
        /// <param name="data">The data sent from JavaScript.</param>
        void PerformAction(Effect element, object data);
    }
}
