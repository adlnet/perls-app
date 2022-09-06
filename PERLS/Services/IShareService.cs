using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PERLS.Services
{
    /// <summary>
    /// A service for sharing files. At the time of this writing, we only use this to share certificates on iOS.
    /// </summary>
    public interface IShareService
    {
        /// <summary>
        /// Request to share the given file request.
        /// </summary>
        /// <param name="request">The request to share.</param>
        /// <returns>An awaitable task.</returns>
        Task RequestAsync(ShareFileRequest request);
    }
}
