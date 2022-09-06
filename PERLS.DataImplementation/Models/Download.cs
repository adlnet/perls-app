using System;
using Float.FileDownloader;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The download implementation.
    /// </summary>
    public class Download : IDownload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Download"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="status">The status.</param>
        /// <param name="dismissableIfFullCache">The dismissable state.</param>
        public Download(IItem item, DownloadStatus status, bool dismissableIfFullCache)
        {
            DownloadItem = item ?? throw new ArgumentNullException(nameof(item));
            Status = status;
            DismissableIfFullCache = dismissableIfFullCache;
        }

        /// <inheritdoc/>
        public IItem DownloadItem { get; set; }

        /// <inheritdoc/>
        public DownloadStatus Status { get; set; }

        /// <inheritdoc/>
        public bool DismissableIfFullCache { get; set; }

        /// <inheritdoc/>
        public bool Equals(IDownload other)
        {
            if (other == null)
            {
                return false;
            }

            return DownloadItem.Equals(other.DownloadItem);
        }
    }
}
