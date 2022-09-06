using System;
using Float.FileDownloader;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The download interface.
    /// </summary>
    public interface IDownload : IEquatable<IDownload>
    {
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        IItem DownloadItem { get; }

        /// <summary>
        /// Gets the download status.
        /// </summary>
        /// <value>
        /// The download status.
        /// </value>
        DownloadStatus Status { get; }

        /// <summary>
        /// Gets a value indicating whether this download is dismissable if the cache is full.
        /// </summary>
        /// <value>
        /// A value indicating whether this download is dismissable if the cache is full.
        /// </value>
        bool DismissableIfFullCache { get; }
    }
}
