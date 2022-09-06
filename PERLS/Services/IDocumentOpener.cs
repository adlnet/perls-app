using System.Threading.Tasks;
using PERLS.Data.Definition;

namespace PERLS.Services
{
    /// <summary>
    /// A document opener interface to work similarly to Xamarin.Essentials' Launcher.
    /// </summary>
    public interface IDocumentOpener
    {
        /// <summary>
        /// Opens the file.
        /// </summary>
        /// <param name="file">The file to be opened.</param>
        /// <returns>The represented task.</returns>
        Task OpenAsync(IFile file);
    }
}
