using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Float.Core.Analytics;
using Float.Core.Definitions;
using Float.Core.Events;
using Float.FileDownloader;
using Float.TinCan.ActivityLibrary;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Extensions;
using PERLS.DataImplementation.Models;
using PERLS.DataImplementation.Providers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// Downloads supporting files required by a Lesson (e.g. for offline access).
    /// </summary>
    public class ResourceDownloader : IDownloaderService
    {
        const string CacheKey = "cache-key";
        readonly ICacheStorage cache;
        readonly DiskProviderCache fileSizeCache = new DiskProviderCache("resource-downloader-filesize", false);
        readonly IDictionary<Guid, DownloadStatus> downloads = new ConcurrentDictionary<Guid, DownloadStatus>();
        readonly ResourceConstrainedActionQueue<IDownload> backgroundDownloadQueue;

        static ResourceDownloader()
        {
            FileStorage.ApplicationDataDirectory = FileSystem.CacheDirectory;
            FileStorage.PackagedContentDirectory = Path.Combine(FileStorage.ApplicationDataDirectory, "html");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDownloader"/> class.
        /// </summary>
        public ResourceDownloader()
        {
            cache = new DiskProviderCache("resource-downloader", false);
            backgroundDownloadQueue = new ResourceConstrainedActionQueue<IDownload>(DownloadItemToPathInBackground)
            {
                HasSufficientResources = () =>
                {
                    return (Battery.PowerSource != BatteryPowerSource.Battery || Battery.ChargeLevel > 0.3 || Battery.ChargeLevel == -1)
                        && Battery.EnergySaverStatus == EnergySaverStatus.Off
                        && Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi);
                },
            };
        }

        /// <inheritdoc />
        public event EventHandler<TypedEventArgs<DownloadStatus>> OnDownloadStarted;

        /// <inheritdoc />
        public IEnumerable<IItem> DownloadedItems => cache.Get<Dictionary<Guid, IItem>>(CacheKey)?.Values ?? Enumerable.Empty<IItem>();

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<Guid, DownloadStatus>> ActiveTrackedDownloads => downloads.Where(pair => pair.Value.State == DownloadStatus.DownloadState.Downloading);

        /// <inheritdoc />
        public async Task<Uri> DownloadFileToPath(IFile remoteResource, DownloadStatus downloadStatus, bool dismissableIfFullCache = false)
        {
            if (remoteResource == null)
            {
                throw new ArgumentNullException(nameof(remoteResource));
            }

            // If this file has previously been downloaded, see if we already know the ETag for it.
            if (remoteResource.IsDownloaded && string.IsNullOrEmpty(remoteResource.ETag))
            {
                var existingFile = DownloadedItems
                    .OfType<IPackagedContent>()
                    .Select(content => content.PackageFile)
                    .FirstOrDefault(file => file?.Url == remoteResource.Url);
                remoteResource.ETag = existingFile?.ETag;
            }

            OnDownloadStarted?.Invoke(this, new TypedEventArgs<DownloadStatus>(downloadStatus));
            var provider = DependencyService.Get<IRemoteFileProvider>();
            var destination = new Uri(remoteResource.LocalPath);
            var response = await FileDownloadRequest.DownloadFile(provider, remoteResource, destination, downloadStatus, DependencyService.Get<IRemoteFileProcessor>()).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.NotModified)
            {
                remoteResource.ETag = response.Headers?.ETag?.Tag;
            }

            var fileSize = response.Content.Headers.ContentLength;
            var process = DependencyService.Get<IPlatformFileProcessor>();

            // If we'll be above our cache size we may want to not download it.
            if (process.TotalCacheSize + fileSize > process.MaximumCacheSize && dismissableIfFullCache)
            {
                downloadStatus.CancelDownload();
            }

            return destination;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Uri>> DownloadItemToPath(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return await DownloadItemToPath(item, new DownloadStatus(item.Name));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Uri>> DownloadItemToPath(IItem item, DownloadStatus downloadStatus, bool dismissableIfFullCache = false)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (downloadStatus == null)
            {
                throw new ArgumentNullException(nameof(downloadStatus));
            }

            TrackDownload(item, downloadStatus);

            if (!DownloadedItems.Contains(item))
            {
                AddDownloadedItem(item); // Tracks the download.
            }

            var downloadableFile = item switch
            {
                IPackagedContent content => content.PackageFile,
                IDocument document => document.DownloadableFile,
                _ => throw new NotImplementedException(),
            };

            try
            {
                var uri = await DownloadFileToPath(downloadableFile, downloadStatus, dismissableIfFullCache);
                AddDownloadedItem(item); // Saves ETag to cache.
                CleanDownloadList(item.Id);
                return new List<Uri> { uri };
            }
            catch (DownloadException e) when (e.Response?.StatusCode == HttpStatusCode.Forbidden
                                      || e.Response?.StatusCode == HttpStatusCode.NotFound
                                      || e.Response?.StatusCode == HttpStatusCode.Gone
                                      || downloadableFile?.IsDownloaded == false)
            {
                RemoveDownloadedItem(item);
                throw e;
            }
        }

        /// <inheritdoc />
        public void RemoveDownloadedItem(IItem item)
        {
            backgroundDownloadQueue.Cancel((arg) => arg.DownloadItem.Id == item.Id);

            if (downloads.ContainsKey(item.Id))
            {
                downloads[item.Id].CancelDownload();
            }

            var list = cache.Get<Dictionary<Guid, IItem>>(CacheKey) ?? new Dictionary<Guid, IItem>();
            list.Remove(item.Id);
            cache.Put(CacheKey, list);
            CleanDownloadList(item.Id);

            if (item is IPackagedContent content && content.PackageFile is IPackageFile file && file.IsDownloaded)
            {
                Directory.Delete(content.PackageFile.LocalExtractedPath, true);
            }
            else if (item is IDocument document && document.File.IsDownloaded)
            {
                System.IO.File.Delete(document.File.LocalPath);
            }

            if (item is IDownloadable downloadable)
            {
                var key = downloadable.DownloadableFile.Url.ToString();

                if (fileSizeCache.ContainsKey(key))
                {
                    fileSizeCache.Delete(key);
                }
            }
        }

        /// <inheritdoc />
        public void AddDownloadedItem(IItem item)
        {
            var list = cache.Get<Dictionary<Guid, IItem>>(CacheKey) ?? new Dictionary<Guid, IItem>();
            list[item.Id] = item;
            cache.Put(CacheKey, list);
            backgroundDownloadQueue.Cancel((arg) => arg.DownloadItem.Id == item.Id);
        }

        /// <inheritdoc />
        public bool GetItemDownloadStatus(IItem item) => item switch
        {
            IPackagedContent content => content.PackageFile?.IsDownloaded == true,
            IDocument document => document.File?.IsDownloaded == true,
            _ => false,
        };

        /// <inheritdoc />
        public void TrackDownload(IItem item, DownloadStatus status)
        {
            downloads[item.Id] = status;
            OnDownloadStarted?.Invoke(item, new TypedEventArgs<DownloadStatus>(status));
        }

        /// <inheritdoc />
        public void CheckForUpdates()
        {
            // Each time we check for updates, we're going to start the update process in a random place.
            // If we always process the list in the same order, front-to-back, then items at the back will
            // be at a disadvantage. A better approach here would be to keep track of the date when an item
            // was last updated--this would allow us to make two improvements here:
            // a) sort by oldest
            // b) skip items that were recently checked
            var itemsToUpdate = DownloadedItems.RotateRandomly();
            DownloadItemsInBackground(itemsToUpdate);
        }

        /// <inheritdoc />
        public Task DownloadItemsInBackground(IEnumerable<IItem> items, bool dismissableIfFullCache = false)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var resourceDependency = DependencyService.Get<IPlatformFileProcessor>();
            if (resourceDependency.TotalCacheSize > (resourceDependency.MaximumCacheSize * .9) && dismissableIfFullCache)
            {
                return backgroundDownloadQueue.Start();
            }

            foreach (var item in items)
            {
                backgroundDownloadQueue.Enqueue(new Download(item, null, dismissableIfFullCache));
            }

            return backgroundDownloadQueue.Start();
        }

        /// <inheritdoc />
        public void PauseBackgroundDownloads()
        {
            backgroundDownloadQueue.Stop();
        }

        /// <inheritdoc />
        public void ResumeBackgroundDownloads()
        {
            backgroundDownloadQueue.Start();
        }

        /// <inheritdoc />
        public void RemoveAllDownloads()
        {
            backgroundDownloadQueue.Clear();

            var count = downloads.Count();

            foreach (var activeDownload in downloads.Values)
            {
                activeDownload.CancelDownload();
            }

            downloads.Clear();

            foreach (var downloadedItem in DownloadedItems)
            {
                try
                {
                    RemoveDownloadedItem(downloadedItem);
                }
                catch
                {
                }
            }

            DependencyService.Get<AnalyticsService>().TrackEvent("User Deleted Local Content", new Dictionary<string, string>
            {
                { "numberOfItems", $"{count}" },
            });
        }

        /// <inheritdoc />
        public long GetFileSize(Uri url)
        {
            if (fileSizeCache.ContainsKey(url.ToString()))
            {
                return fileSizeCache.Get<long>(url.ToString());
            }

            return -1;
        }

        /// <inheritdoc />
        public void SetFileSize(Uri file, long size)
        {
            fileSizeCache.Put(file.ToString(), size);
        }

        static async Task<long> RecursiveSize(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return 0;
            }

            var topLevelFiles = Directory.EnumerateFiles(path).Select(file => System.IO.File.Exists(file) ? new FileInfo(file).Length : 0);
            var recursiveSizeTasks = Directory.EnumerateDirectories(path).Select(dir => RecursiveSize(dir));
            var otherFiles = await Task.WhenAll(recursiveSizeTasks).ConfigureAwait(false);

            return topLevelFiles.Concat(otherFiles).AsParallel().Sum();
        }

        async Task<IEnumerable<Uri>> DownloadItemToPathInBackground(IDownload download)
        {
            if (download == null)
            {
                throw new ArgumentNullException(nameof(download));
            }

            return await DownloadItemToPath(download.DownloadItem, new DownloadStatus(download.DownloadItem.Name), true);
        }

        void CleanDownloadList(Guid id)
        {
            if (downloads.ContainsKey(id))
            {
                downloads.Remove(id);
            }
        }
    }
}
