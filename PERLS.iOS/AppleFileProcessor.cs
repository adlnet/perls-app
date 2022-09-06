using System.IO;
using System.Linq;
using Foundation;
using PERLS.Data.Definition.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.iOS
{
    /// <summary>
    /// A file processor for configuring the cache on iOS.
    /// </summary>
    public class AppleFileProcessor : IPlatformFileProcessor
    {
        const string ApplicationSupportFolder = "Application Support";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleFileProcessor"/> class.
        /// </summary>
        public AppleFileProcessor()
        {
        }

        /// <inheritdoc/>
        public string NoBackupFolder => Path.Combine(Path.GetDirectoryName(FileSystem.CacheDirectory), ApplicationSupportFolder, AppInfo.PackageName);

        /// <inheritdoc/>
        public long TotalCacheSize
        {
            get
            {
                var directory = FileSystem.CacheDirectory;
                var subpaths = NSFileManager.DefaultManager.Subpaths(directory);
                var size = subpaths?.ToList().Sum(file => File.Exists($"{directory}/{file}") ? new FileInfo($"{directory}/{file}").Length : 0) ?? 0;
                return size;
            }
        }

        /// <inheritdoc/>
        public long MaximumCacheSize
        {
            get
            {
                using (var fileURL = NSUrl.CreateFileUrl(FileSystem.CacheDirectory, true, null))
                {
                    var key = NSUrl.VolumeAvailableCapacityForOpportunisticUsageKey;
                    var resourceKeys = new NSString[] { key };
                    var resourceDictionary = fileURL.GetResourceValues(resourceKeys, out var error);

                    if (resourceDictionary == null || error != null)
                    {
                        error?.Dispose();
                        return long.MaxValue;
                    }

                    error?.Dispose();

                    var result = resourceDictionary[key];

                    if (result is NSNumber number)
                    {
                        return number.LongValue;
                    }

                    return long.MaxValue;
                }
            }
        }

        /// <inheritdoc/>
        public void NotifyDownloaded(string path)
        {
            if (Directory.Exists(path))
            {
                // we don't want iOS to back up package content to iCloud
                using (var nstrue = new NSNumber(true))
                {
                    using (var folder = NSUrl.FromFilename(path))
                    {
                        folder.SetResource(NSUrl.IsExcludedFromBackupKey, nstrue);
                    }
                }
            }
        }
    }
}
