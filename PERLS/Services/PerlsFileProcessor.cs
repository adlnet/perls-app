using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Float.FileDownloader;
using Float.TinCan.ActivityLibrary;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// A file processor that unzips files to local storage.
    /// </summary>
    public class PerlsFileProcessor : IRemoteFileProcessor
    {
        readonly Action<Uri, long> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerlsFileProcessor"/> class.
        /// </summary>
        public PerlsFileProcessor()
        {
            this.store = DependencyService.Get<IDownloaderService>().SetFileSize;
        }

        /// <inheritdoc />
        public async Task ProcessDownload(IRemoteFile file, string downloadPath, HttpResponseMessage response)
        {
            if (Path.GetExtension(downloadPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                var destination = Path.Combine(FileStorage.PackagedContentDirectory, Path.GetFileNameWithoutExtension(downloadPath));
                var path = await FileUnzipper.UnzipFile(downloadPath, destination).ConfigureAwait(false);
                var fullPath = Path.Combine(Path.GetDirectoryName(downloadPath), path);
                DependencyService.Get<IPlatformFileProcessor>()?.NotifyDownloaded(fullPath);
                var size = await RecursiveSize(destination).ConfigureAwait(false);
                store.Invoke(file.Url, size);
            }
            else
            {
                DependencyService.Get<IPlatformFileProcessor>()?.NotifyDownloaded(downloadPath);
                var size = File.Exists(downloadPath) ? new FileInfo(downloadPath).Length : 0;
                store.Invoke(file.Url, size);
            }
        }

        static async Task<long> RecursiveSize(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return 0;
            }

            var topLevelFiles = Directory.EnumerateFiles(path).Select(file => File.Exists(file) ? new FileInfo(file).Length : 0);
            var recursiveSizeTasks = Directory.EnumerateDirectories(path).Select(dir => RecursiveSize(dir));
            var otherFiles = await Task.WhenAll(recursiveSizeTasks).ConfigureAwait(false);

            return topLevelFiles.Concat(otherFiles).AsParallel().Sum();
        }
    }
}
