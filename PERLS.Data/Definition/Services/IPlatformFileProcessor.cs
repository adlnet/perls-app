namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// Defines a platform-implemented service that handles native calls when downloads complete.
    /// </summary>
    public interface IPlatformFileProcessor
    {
        /// <summary>
        /// Gets the path to a folder that will not be backed up to the cloud, or equivalent.
        /// </summary>
        /// <value>The path to the folder for large cached content.</value>
        string NoBackupFolder { get; }

        /// <summary>
        /// Gets the total cache size.
        /// </summary>
        /// <value>
        /// The total cache size.
        /// </value>
        long TotalCacheSize { get; }

        /// <summary>
        /// Gets the maximum cache size.
        /// </summary>
        /// <value>
        /// The maximum cache size.
        /// </value>
        long MaximumCacheSize { get; }

        /// <summary>
        /// Call when the shared code has finished downloading (and, if necessary, unzipping) a file.
        /// </summary>
        /// <remarks>
        /// For example, the platform implementation may want to add flags to a file for OS cloud sync operations.
        /// </remarks>
        /// <param name="path">The path to the file.</param>
        void NotifyDownloaded(string path);
    }
}
