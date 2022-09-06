using System.Threading.Tasks;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Push token provider.
    /// </summary>
    public interface IPushTokenProvider
    {
        /// <summary>
        /// Sends the push token to the server.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Always returning true, leaving in ability for false.</returns>
        Task<bool> SendPushToken(string token);

        /// <summary>
        /// Sends a delete request to delete the push token from the server.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Always returning true, leaving in ability for false.</returns>
        Task<bool> SendDeletePushToken(string token);
    }
}
