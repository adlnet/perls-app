using System;
using System.Net.Http;
using Foundation;
using PERLS.Data;
using PERLS.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(NativeHttpClientHandler))]

namespace PERLS.iOS
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

            return new NSUrlSessionHandler(CreateConfig(config));
        }

        static NSUrlSessionConfiguration CreateConfig(ClientHandlerConfiguration config)
        {
            // Stolen from Source
            var configuration = NSUrlSessionConfiguration.DefaultSessionConfiguration;
            configuration.TimeoutIntervalForRequest = config.ReadTimeout.Seconds; // Apple Default: 60
            configuration.TimeoutIntervalForResource = config.ResourceTimeout.Seconds; // Apple Default: 7 days
            configuration.HttpShouldSetCookies = config.AllowCookies;
            return configuration;
        }
    }
}
