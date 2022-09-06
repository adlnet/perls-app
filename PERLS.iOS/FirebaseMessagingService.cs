using System.Threading.Tasks;
using Firebase.CloudMessaging;
using PERLS.Data.Definition;
using PERLS.DataImplementation;
using PERLS.Services;
using UserNotifications;
using Xamarin.Forms;

namespace PERLS.iOS
{
    /// <summary>
    /// The Firebase iOS Messaging Service.
    /// </summary>
    public class FirebaseMessagingService : BaseFirebaseMessagingService
    {
        /// <inheritdoc />
        public override void RequestPushPermission()
        {
            var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
            UNUserNotificationCenter.Current.RequestAuthorization(
                authOptions,
                (granted, error) =>
                {
                    if (granted)
                    {
                        _ = UpdatePushToken();
                    }
                });
        }

        /// <inheritdoc />
        public override Task<bool> UpdatePushToken(string token = null)
        {
            if (!string.IsNullOrEmpty(Messaging.SharedInstance?.FcmToken) && DependencyService.Get<DrupalAPI>() is DrupalAPI drupalAPI)
            {
                return drupalAPI.SendPushToken(Messaging.SharedInstance.FcmToken);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        /// <inheritdoc/>
        public override Task<bool> DeletePushToken()
        {
            if (!string.IsNullOrEmpty(Messaging.SharedInstance?.FcmToken) && DependencyService.Get<DrupalAPI>() is DrupalAPI drupalAPI)
            {
                return drupalAPI.SendDeletePushToken(Messaging.SharedInstance.FcmToken);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}
