using System;

namespace PERLS.Services
{
    /// <summary>
    /// Defines a service for running Unity within this app.
    /// </summary>
    public interface IUnityService
    {
        /// <summary>
        /// Gets a value indicating whether Unity is initialized.
        /// </summary>
        /// <value>Whether or not Unity is initialized.</value>
        /// <returns><c>true</c> if Unity is initialized, <c>false</c> otherwise.</returns>
        bool IsInitialized { get; }

        /// <summary>
        /// Shows the Unity window.
        /// </summary>
        void ShowUnity();

        /// <summary>
        /// Specify an action to invoke on completion of Unity content.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        void OnCompleted(Action action);

        /// <summary>
        /// Specify an action to invoke on termination (not completion) of Unity content.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        void OnQuit(Action action);

        /// <summary>
        /// Sends a message to a Unity game object.
        /// </summary>
        /// <param name="gameObjectName">The name of the game object to message.</param>
        /// <param name="functionName">The name of the function to invoke.</param>
        /// <param name="message">The message to pass.</param>
        void SendMessage(string gameObjectName, string functionName, string message);
    }
}
