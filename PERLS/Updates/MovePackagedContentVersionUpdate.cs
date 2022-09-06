using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Float.FileDownloader;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Updates
{
    /// <summary>
    /// Moves packaged content from AppDataDirectory to PackagedContentDirectory.
    /// </summary>
    public class MovePackagedContent : VersionUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovePackagedContent"/> class.
        /// </summary>
        public MovePackagedContent()
        {
        }

        IEnumerable<IDownloadable> DownloadedItems => DependencyService.Get<IDownloaderService>().DownloadedItems.OfType<IDownloadable>().Distinct();

        /// <inheritdoc/>
        public override Task Update()
        {
            return Task.Run(() =>
            {
                var files = from item in DownloadedItems
                                 orderby item.Name
                                 select item.DownloadableFile;
                foreach (var file in files)
                {
                    if (file is not IPackageFile)
                    {
                        continue;
                    }

                    var zip = ".ZIP";
                    var previousLocation = file.LocalPath.Remove(file.LocalPath.Length - zip.Length);
                    if (Directory.Exists(previousLocation))
                    {
                        var newLocation = ((IPackageFile)file).LocalExtractedPath;
                        MoveDirectory(previousLocation, newLocation);
                    }
                }
            });
        }

        /// <summary>
        /// Moves directory and files.
        /// Stolen from https://stackoverflow.com/questions/2553008/directory-move-doesnt-work-file-already-exist.
        /// </summary>
        /// <param name="source">The source directory.</param>
        /// <param name="target">The target directory.</param>
        static void MoveDirectory(string source, string target)
        {
            var sourcePath = source.TrimEnd('\\', ' ');
            var targetPath = target.TrimEnd('\\', ' ');
            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                 .GroupBy(s => Path.GetDirectoryName(s));

            var downloadedItems = DependencyService.Get<IDownloaderService>().DownloadedItems.OfType<IDownloadable>().Distinct();

            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);
                foreach (var file in folder)
                {
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile))
                    {
                        File.Delete(targetFile);
                    }

                    File.Move(file, targetFile);

                    if (downloadedItems.FirstOrDefault((arg) => arg.DownloadableFile.LocalPath == file) is IDownloadable remoteFile)
                    {
                        DependencyService.Get<IRemoteFileProcessor>().ProcessDownload(remoteFile.DownloadableFile, targetFile, null);
                    }
                }
            }

            Directory.Delete(source, true);
        }
    }
}
