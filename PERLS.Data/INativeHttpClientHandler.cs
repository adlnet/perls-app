using System.Net.Http;

namespace PERLS.Data
{
    /// <summary>
    /// Creates a native handler with configuration for HTTP Clients.
    /// </summary>
    public interface INativeHttpClientHandler
    {
        /// <summary>
        /// Creates the handler.
        /// </summary>
        /// <param name="config">Configuration for the http client handler.</param>
        /// <returns>Returns a native message handler.</returns>
        HttpMessageHandler CreateHandler(ClientHandlerConfiguration config);
    }
}
