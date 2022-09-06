using System;
using System.IO;
using Float.TinCan.ActivityLibrary;
using PERLS.Data.Definition;
using Xamarin.Essentials;

namespace PERLS.Data
{
    /// <summary>
    /// Represents a file resource that is embedded in another resource.
    /// </summary>
    /// <remarks>
    /// The use case here is to download files that were embedded in another piece of content.
    /// Ultimately the presence of or how temporary files are accessed are an implementation detail
    /// and should be in the `DataImplementation` project.
    /// </remarks>
    public class EmbeddedFile : IFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedFile"/> class.
        /// </summary>
        /// <param name="url">The URL to the file.</param>
        public EmbeddedFile(Uri url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Name = Path.GetFileName(url.LocalPath);
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string LocalPath => Path.Combine(FileStorage.ApplicationDataDirectory, Name);

        /// <inheritdoc />
        public bool IsDownloaded => File.Exists(LocalPath);

        /// <inheritdoc />
        public Uri Url { get; }

        /// <inheritdoc />
        public string ETag { get; set; }
    }
}
