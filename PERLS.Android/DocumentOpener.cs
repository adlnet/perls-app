using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using PERLS.Data.Definition;
using PERLS.Droid;
using PERLS.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(DocumentOpener))]

namespace PERLS.Droid
{
    /// <summary>
    /// The Android implemtation for IDocumentOpener.
    /// </summary>
    public class DocumentOpener : IDocumentOpener
    {
        /// <inheritdoc/>
        public Task OpenAsync(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return Launcher.OpenAsync(
                new OpenFileRequest(
                    file.Name,
                    new ReadOnlyFile(file.LocalPath)));
        }
    }
}
