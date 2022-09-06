using System;
using Float.Core.Net;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// The Network Connection Dependency Service interface.
    /// </summary>
    public interface INetworkConnectionService : IRemoteProvider
    {
        /// <summary>
        /// Gets the Auth Strategy.
        /// </summary>
        /// <value>The auth strategy.</value>
        IAuthStrategy AuthStrategy { get; }

        /// <summary>
        /// Gets the Base URI for the server.
        /// </summary>
        /// <value>The base URI.</value>
        Uri BaseUri { get; }

        /// <summary>
        /// Retrieve the authorization URL.
        /// </summary>
        /// <param name="requestLocalAuthentication">Requesting local authentication will ask the server to not redirect to single sign-on.</param>
        /// <returns>The URL to use to authenticate the user.</returns>
        Uri GetAuthorizationUrl(bool requestLocalAuthentication = false);

        /// <summary>
        /// Refreshes the AuthStrategy.
        /// </summary>
        void RefreshAuthStrategy();
    }
}
