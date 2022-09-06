using System.Threading.Tasks;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// The Firebase Messaging Service Interface.
    /// </summary>
    public interface IFirebaseMessagingService
    {
        /// <summary>
        /// Request user's permission to receive push notifications.
        /// </summary>
        void RequestPushPermission();

        /// <summary>
        /// Send push token to server.
        /// </summary>
        /// <param name="token">The push token.</param>
        /// <returns>Task for async.</returns>
        Task<bool> UpdatePushToken(string token = null);

        /// <summary>
        /// Delete the push token from the server.
        /// </summary>
        /// <returns>Task for async.</returns>
        Task<bool> DeletePushToken();

        /// <summary>
        /// Check if the app is receiving push notifications, and set up push notifications.
        /// </summary>
        void PerformNotificationCheck();
    }
}
