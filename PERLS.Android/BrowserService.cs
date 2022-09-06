using System;
using System.Threading.Tasks;
using PERLS.Services;
using Xamarin.Essentials;

namespace PERLS.Droid
{
    /// <summary>
    /// The Android implementation of IBrowserService.
    /// </summary>
    public class BrowserService : IBrowserService
    {
        /// <inheritdoc/>
        public Task<bool> OpenBrowser(Uri uri, BrowserLaunchOptions options) => Browser.OpenAsync(uri, options);
    }
}
