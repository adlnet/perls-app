namespace PERLS.Services
{
    /// <summary>
    /// Service to check access to push notifications.
    /// </summary>
    public interface INotificationAccessService
    {
        /// <summary>
        /// Checks if push notifications are enabled.
        /// </summary>
        /// <returns>True if enabled.</returns>
        bool ArePushNotificationsAvailable();
    }
}
