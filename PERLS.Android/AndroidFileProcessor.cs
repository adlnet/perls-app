using System.IO;
using Android.Content;
using Android.OS;
using Android.OS.Storage;
using PERLS.Data.Definition.Services;

namespace PERLS.Droid
{
    /// <summary>
    /// A file processor for configuring the cache on Android.
    /// </summary>
    public class AndroidFileProcessor : IPlatformFileProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidFileProcessor"/> class.
        /// </summary>
        public AndroidFileProcessor()
        {
        }

        /// <inheritdoc />
        public string NoBackupFolder => Android.App.Application.Context.NoBackupFilesDir.AbsolutePath;

        /// <inheritdoc/>
        public long TotalCacheSize
        {
            get
            {
                // StorageManager was introduced in Api Level 26 / O.
                if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                {
                    return long.MinValue;
                }

                if (Android.App.Application.Context is Context context && context.GetSystemService(Context.StorageService) is StorageManager storageManager)
                {
                    return storageManager.GetCacheSizeBytes(StorageManager.UuidDefault);
                }

                return long.MinValue;
            }
        }

        /// <inheritdoc/>
        public long MaximumCacheSize
        {
            get
            {
                // StorageManager was introduced in Api Level 26 / O.
                if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                {
                    return long.MaxValue;
                }

                if (Android.App.Application.Context is Context context && context.GetSystemService(Context.StorageService) is StorageManager storageManager)
                {
                    return storageManager.GetAllocatableBytes(StorageManager.UuidDefault);
                }

                return long.MaxValue;
            }
        }

        /// <inheritdoc />
        public void NotifyDownloaded(string path)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.OMr1)
            {
                // SetCacheBehaviorGroup was added in API 27.
                return;
            }

            if (Directory.Exists(path))
            {
                if (Android.App.Application.Context is Context context)
                {
                    if (context.GetSystemService(Context.StorageService) is StorageManager storageManager)
                    {
                        using (var folder = new Java.IO.File(path))
                        {
                            storageManager.SetCacheBehaviorGroup(folder, true);
                        }
                    }
                }
            }
        }
    }
}
