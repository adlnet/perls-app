using Float.FileDownloader;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Represents a file entity.
    /// </summary>
    public interface IFile : IRemoteFile
    {
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        string Name { get; }

        /// <summary>
        /// Gets the local save path of the file. Ideally this would remain consistent given a filename to help with caching.
        /// </summary>
        /// <value>The local path.</value>
        string LocalPath { get; }

        /// <summary>
        /// Gets a value indicating whether or not the local file is downloaded already.
        /// </summary>
        /// <value><c>true</c> if downloaded, <c>false</c> otherwise.</value>
        bool IsDownloaded { get; }

        /// <summary>
        /// Gets or sets the HTTP entity tag value for this file.
        /// </summary>
        /// <value>The entity tag representing the state/version of this file.</value>
        string ETag { get; set; }
    }
}
