using System.Threading.Tasks;
using Float.Core.Persistence;
using PERLS.Data.Definition;
using PERLS.DataImplementation;
using PERLS.Services;
using Xamarin.Forms;

namespace PERLS.Droid
{
    /// <summary>
    /// The Firebase Android Messaging Service.
    /// </summary>
    public class FirebaseMessagingService : BaseFirebaseMessagingService
    {
        const string FirebaseTokenKey = "fbtkn";

        /// <inheritdoc />
        public override void RequestPushPermission()
        {
            _ = UpdatePushToken();
        }

        /// <inheritdoc />
        public override async Task<bool> UpdatePushToken(string token = null)
        {
            var storageToken = await DependencyService.Get<ISecureStore>().GetAsync(FirebaseTokenKey).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(token) && storageToken != token)
            {
                DependencyService.Get<ISecureStore>().Put(FirebaseTokenKey, token);
            }

            if (string.IsNullOrEmpty(token))
            {
                token = storageToken;
            }

            if (!string.IsNullOrEmpty(token) && DependencyService.Get<DrupalAPI>() is DrupalAPI drupalAPI)
            {
                return await drupalAPI.SendPushToken(token).ConfigureAwait(false);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override async Task<bool> DeletePushToken()
        {
            var storageToken = await DependencyService.Get<ISecureStore>().GetAsync(FirebaseTokenKey).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(storageToken) && DependencyService.Get<DrupalAPI>() is DrupalAPI drupalAPI)
            {
                return await drupalAPI.SendDeletePushToken(storageToken).ConfigureAwait(false);
            }
            else
            {
                return false;
            }
        }
    }
}
