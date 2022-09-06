using Float.Core.L10n;
using Float.Core.Notifications;
using PERLS.Data;

namespace PERLS.Extensions
{
    /// <summary>
    /// Convenience methods for reported expected error scenarios.
    /// </summary>
    public static class NotificationHandlerExtensions
    {
        /// <summary>
        /// Notify the user that a request couldn't be fulfilled because the device is offline.
        /// </summary>
        /// <param name="handler">The handler for the error.</param>
        public static void NotifyDeviceOffline(this INotificationHandler handler)
        {
            handler.NotifyError(Strings.DefaultErrorTitle, Localize.String("NoInternetMessage"));
        }
    }
}
