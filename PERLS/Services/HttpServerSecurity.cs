using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;

namespace PERLS.Services
{
    /// <summary>
    /// Generates, stores, and verifies hashes used to authenticate requests from
    /// in app web views.
    /// </summary>
    public class HttpServerSecurity
    {
        /// <summary>
        /// Header key value to inject in webview and check in server.
        /// </summary>
        public const string SecurityHeaderKey = "X-SL-Key";

        private readonly HashSet<string> reservedKeys = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServerSecurity"/> class.
        /// </summary>
        public HttpServerSecurity()
        {
        }

        /// <summary>
        /// Checks if the key is allowed. Removes key if it is in the lookup
        /// table and throws an error if allowed.
        /// </summary>
        /// <param name="cookies">The cookies to which to lookup a value.</param>
        /// <returns><c>True</c> if the key is valid.</returns>
        public bool Lookup(CookieCollection cookies)
        {
            if (cookies == null)
            {
                return false;
            }

            foreach (Cookie cookie in cookies)
            {
                var key = cookie.Value;
                if (reservedKeys.Contains(key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generates a key to verify against.
        /// </summary>
        /// <param name="serverUri">The Uriof the server.</param>
        /// <returns>Returns the key to inject into the web view.</returns>
        public Cookie GenerateCookie(Uri serverUri)
        {
            Contract.Assert(!string.IsNullOrEmpty(serverUri?.Host));
            var key = Guid.NewGuid().ToString();
            var cookie = new Cookie(SecurityHeaderKey, key, "/", serverUri.Host);
            reservedKeys.Add(key);
            return cookie;
        }

        /// <summary>
        /// Removes checks cookies and removes matching key from reserved keys.
        /// </summary>
        /// <param name="cookie">The cookie containing the key.</param>
        public void RemoveCookie(Cookie cookie)
        {
            if (cookie == null)
            {
                return;
            }

            var key = cookie.Value;
            if (reservedKeys.Contains(key))
            {
                reservedKeys.Remove(key);
            }
        }
    }
}
