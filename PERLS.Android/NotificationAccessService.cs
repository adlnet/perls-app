using AndroidX.Core.App;
using PERLS.Services;

namespace PERLS.Droid
{
    /// <summary>
    /// Android implementation of INotificationAccessService.
    /// </summary>
    public class NotificationAccessService : INotificationAccessService
    {
        /// <inheritdoc/>
        public bool ArePushNotificationsAvailable()
        {
            return NotificationManagerCompat.From(Android.App.Application.Context).AreNotificationsEnabled();
        }
    }
}
