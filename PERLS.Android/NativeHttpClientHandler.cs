using System;
using System.Net.Http;
using PERLS.Data;
using PERLS.Droid;
using Xamarin.Android.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(NativeHttpClientHandler))]

namespace PERLS.Droid
{
    /// <summary>
    /// Default Http client handler.
    /// </summary>
    public class NativeHttpClientHandler : INativeHttpClientHandler
    {
        /// <inheritdoc/>
        public HttpMessageHandler CreateHandler(ClientHandlerConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return new AndroidClientHandler
            {
                UseCookies = config.AllowCookies,
                ReadTimeout = config.ReadTimeout,
                ConnectTimeout = config.ConnectionTimeout,
            };
        }
    }
}
