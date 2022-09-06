using PERLS.Services;
using UIKit;

namespace PERLS.iOS
{
    /// <summary>
    /// iOS implementation of INotificationAccessService.
    /// </summary>
    public class NotificationAccessService : INotificationAccessService
    {
        /// <inheritdoc/>
        public bool ArePushNotificationsAvailable()
        {
            var types = UIApplication.SharedApplication.CurrentUserNotificationSettings.Types;
            return types.HasFlag(UIUserNotificationType.Alert);
        }
    }
}
