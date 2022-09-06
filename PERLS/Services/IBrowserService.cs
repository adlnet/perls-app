using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PERLS.Services
{
    /// <summary>
    /// A browser (web/file) service.
    /// </summary>
    public interface IBrowserService
    {
        /// <summary>
        /// Opens a browser window pointing towards the provided Uri.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="options">The launch options.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<bool> OpenBrowser(Uri uri, BrowserLaunchOptions options);
    }
}
