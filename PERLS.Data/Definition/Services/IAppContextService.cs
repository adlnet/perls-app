using System;
using System.Threading.Tasks;
using TinCan;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// Contextual information about the app.
    /// </summary>
    public interface IAppContextService
    {
        /// <summary>
        /// Gets the app's display name.
        /// </summary>
        /// <value>The app display name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the app's identifier (e.g. the package name).
        /// </summary>
        /// <value>The app identifier.</value>
        string Identifier { get; }

        /// <summary>
        /// Gets the build number currently installed.
        /// </summary>
        /// <value>The app build number.</value>
        string BuildNumber { get; }

        /// <summary>
        /// Gets the version currently installed.
        /// </summary>
        /// <value>The app version.</value>
        string Version { get; }

        /// <summary>
        /// Gets the user agent to be used for the WebViews when viewing content.
        /// </summary>
        /// <value>The user agent suffix.</value>
        string UserAgentSuffix { get; }

        /// <summary>
        /// Gets the current package identifier.
        /// </summary>
        /// <value>The package identifier.</value>
        string PackageIdentifier { get; }

        /// <summary>
        /// Gets the Server using the configuration's base URL.
        /// </summary>
        /// <value>The server.</value>
        Uri Server { get; }

        /// <summary>
        /// Gets a value indicating whether the user is authenticated.
        /// </summary>
        /// <value><c>true</c> if the user is authenticated, <c>false</c> otherwise.</value>
        bool IsLearnerAuthenticated { get; }

        /// <summary>
        /// Gets the currently authenticated learner.
        /// </summary>
        /// <value>The current learner.</value>
        ILearner CurrentLearner { get; }

        /// <summary>
        /// Gets an xAPI agent for the current learner.
        /// </summary>
        /// <value>The current learner agent.</value>
        Agent LearnerAgent { get; }

        /// <summary>
        /// Gets an xAPI agent for the current application.
        /// </summary>
        /// <value>The current system agent.</value>
        Agent SystemAgent { get; }

        /// <summary>
        /// Refreshes the current learner profile.
        /// This should only throw if the learner needs to log in again.
        /// </summary>
        /// <returns>The learner profile.</returns>
        Task<ILearner> RefreshLearnerProfile();

        /// <summary>
        /// Logs out the current learner.
        /// </summary>
        void Logout();
    }
}
