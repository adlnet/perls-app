using System.Threading.Tasks;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// The Firebase messaging service for the app.
    /// </summary>
    public abstract class BaseFirebaseMessagingService : IFirebaseMessagingService
    {
        /// <inheritdoc />
        public abstract void RequestPushPermission();

        /// <inheritdoc />
        public abstract Task<bool> UpdatePushToken(string token = null);

        /// <inheritdoc/>
        public abstract Task<bool> DeletePushToken();

        /// <inheritdoc />
        public void PerformNotificationCheck()
        {
            UpdatePushToken();
        }
    }
}
