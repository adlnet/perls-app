using Foundation;
using PERLS.Data;
using PERLS.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SettingsService))]

namespace PERLS.iOS
{
    /// <summary>
    /// The iOS implementation of the Settings Service.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        /// <inheritdoc/>
        public bool CanOpenSettings()
        {
            return true;
        }

        /// <inheritdoc/>
        public void OpenAppSettings()
        {
            NSUrl url = new NSUrl(UIApplication.OpenSettingsUrlString);
            UIApplication.SharedApplication.OpenUrl(url);
            url.Dispose();
        }
    }
}
