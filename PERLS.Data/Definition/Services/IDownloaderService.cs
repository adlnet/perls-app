using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Float.Core.Events;
using Float.FileDownloader;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// A service that provides means to download files from a remote server to store locally.
    /// </summary>
    public interface IDownloaderService
    {
        /// <summary>
        /// An event that is triggered when a new download status item is available.
        /// </summary>
        event EventHandler<TypedEventArgs<DownloadStatus>> OnDownloadStarted;

        /// <summary>
        /// Gets a list of items that have been cached via <see cref="AddDownloadedItem(IItem)"/>.
        /// </summary>
        /// <value>All downloaded items.</value>
        IEnumerable<IItem> DownloadedItems { get; }

        /// <summary>
        /// Gets all active tracked downloads.
        /// </summary>s
        /// <value>An enumerable list of id/status pairs.</value>
        IEnumerable<KeyValuePair<Guid, DownloadStatus>> ActiveTrackedDownloads { get; }

        /// <summary>
        /// Downloads a file to a path and returns the path it was saved to.
        /// </summary>
        /// <param name="remoteResource">The remote resource.</param>
        /// <param name="downloadStatus">The download status.</param>
        /// <param name="dismissableIfFullCache">If the download is dismissable if the cache is full.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Uri> DownloadFileToPath(IFile remoteResource, DownloadStatus downloadStatus, bool dismissableIfFullCache = false);

        /// <summary>
        /// Downloads all files for an item to a path and returns the paths the files were saved to.
        /// </summary>
        /// <param name="item">The item whose files should be downloaded.</param>
        /// <returns>An enumerable list of resource identifiers.</returns>
        Task<IEnumerable<Uri>> DownloadItemToPath(IItem item);

        /// <summary>
        /// Downloads all files for an item to a path and returns the paths the files were saved to.
        /// </summary>
        /// <param name="item">The item whose files should be downloaded.</param>
        /// <param name="downloadStatus">The download status.</param>
        /// <param name="dismissableIfFullCache">If the download is dismissable if the cache is full.</param>
        /// <returns>An enumerable list of resource identifiers.</returns>
        Task<IEnumerable<Uri>> DownloadItemToPath(IItem item, DownloadStatus downloadStatus, bool dismissableIfFullCache = false);

        /// <summary>
        /// Get a status indication for a given item.
        /// </summary>
        /// <param name="item">The item for which to get the status.</param>
        /// <returns><c>true</c> if the item is downloaded, <c>false</c> otherwise.</returns>
        bool GetItemDownloadStatus(IItem item);

        /// <summary>
        /// Add an item to the cache of downloaded items.
        /// </summary>
        /// <param name="item">The item that has been downloaded.</param>
        /// <remarks>
        /// TODO: This method is misnamed; it really should be something like "TrackDownloadedItem".
        /// </remarks>
        void AddDownloadedItem(IItem item);

        /// <summary>
        /// Remove an item from the cache of downloaded items.
        /// </summary>
        /// <param name="item">The item that is no longer downloaded.</param>
        void RemoveDownloadedItem(IItem item);

        /// <summary>
        /// Add a download status to the list of tracked downloads, keyed by the item's unique ID.
        /// </summary>
        /// <param name="id">The unique ID for the downloading item.</param>
        /// <param name="status">The download status for the item.</param>
        void TrackDownload(IItem id, DownloadStatus status);

        /// <summary>
        /// Start checking for updates on currently downloaded items.
        /// </summary>
        void CheckForUpdates();

        /// <summary>
        /// Downloads a batch of items in the background.
        /// </summary>
        /// <param name="items">The batch of items to download.</param>
        /// <param name="dismissableIfFullCache">If this can be dismissed if the cache is full.</param>
        /// <returns>An awaitable task for when all items have been downloaded.</returns>
        Task DownloadItemsInBackground(IEnumerable<IItem> items, bool dismissableIfFullCache = false);

        /// <summary>
        /// Pauses the background download queue.
        /// </summary>
        void PauseBackgroundDownloads();

        /// <summary>
        /// Resumes the background download queue.
        /// </summary>
        void ResumeBackgroundDownloads();

        /// <summary>
        /// Removes all downloads and cancels all pending downloads.
        /// </summary>
        void RemoveAllDownloads();

        /// <summary>
        /// Returns the size previously computed on download for an item, if one exists.
        /// </summary>
        /// <param name="url">The remote location of the item whose size should be retrieved.</param>
        /// <returns>The size, in bytes. If no file size was found, this returns -1.</returns>
        long GetFileSize(Uri url);

        /// <summary>
        /// Sets the size on item download.
        /// </summary>
        /// <param name="url">The remote location of the file; used as a unique key.</param>
        /// <param name="size">The size, in bytes.</param>
        void SetFileSize(Uri url, long size);
    }
}
