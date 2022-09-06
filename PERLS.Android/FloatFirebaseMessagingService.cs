using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Firebase.Messaging;
using PERLS.Data;
using Xamarin.Forms;

namespace PERLS.Droid
{
    /// <summary>
    /// Implement FirebaseMessagingService.
    /// </summary>
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FloatFirebaseMessagingService : Firebase.Messaging.FirebaseMessagingService
    {
        internal const string FirebaseChannelId = "firebase_notification_channel";
        internal const int FirebaseNotificationId = 100;

        /// <summary>
        /// Handle receipt of a new token.
        /// </summary>
        /// <param name="newToken"> The new token.</param>
        public override void OnNewToken(string newToken)
        {
            base.OnNewToken(newToken);
            if (newToken != null)
            {
                MessagingCenter.Send(this, Strings.FirebaseTokenMessage, newToken);
            }
        }

        /// <summary>
        /// Handle receipt of a message when the app is in foreground.
        /// </summary>
        /// <param name="message">The message payload.</param>
        public override void OnMessageReceived(RemoteMessage message)
        {
            if (message == null)
            {
                DependencyService.Get<Float.Core.Analytics.AnalyticsService>().TrackException(new ArgumentNullException(nameof(message)));
                return;
            }

            var title = message.GetNotification().Title;
            var body = message.GetNotification().Body;
            SendNotification(title, body, message.Data);
        }

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="messageBody">The notification message.</param>
        /// <param name="data">The notification data.</param>
        void SendNotification(string title, string messageBody, IDictionary<string, string> data)
        {
            using (var intent = new Intent(this, typeof(MainActivity)))
            {
                intent.AddFlags(ActivityFlags.ClearTop);

                if (data != null)
                {
                    foreach (var key in data.Keys)
                    {
                        intent.PutExtra(key, data[key]);
                    }
                }

                var pendingIntent = PendingIntent.GetActivity(
                    this,
                    FirebaseNotificationId,
                    intent,
                    PendingIntentFlags.CancelCurrent);

                using (var notificationBuilder = new NotificationCompat.Builder(this, FirebaseChannelId))
                {
                    notificationBuilder.SetSmallIcon(Resource.Drawable.notification_icon)
                        .SetContentTitle(title)
                        .SetContentText(messageBody)
                        .SetAutoCancel(true)
                        .SetDefaults(NotificationCompat.DefaultAll)
                        .SetPriority(NotificationCompat.PriorityHigh)
                        .SetVisibility(NotificationCompat.VisibilityPublic)
                        .SetContentIntent(pendingIntent);

                    var notificationManager = NotificationManagerCompat.From(this);
                    notificationManager.Notify(FirebaseNotificationId, notificationBuilder.Build());
                }
            }
        }
    }
}
