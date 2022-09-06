using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Updates
{
    /// <summary>
    /// Class to handle deleting unmanaged documents.
    /// </summary>
    public class DeleteOrphanedDocumentsVersionUpdate : VersionUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteOrphanedDocumentsVersionUpdate"/> class.
        /// </summary>
        public DeleteOrphanedDocumentsVersionUpdate()
        {
        }

        IEnumerable<IDownloadable> DownloadedItems => DependencyService.Get<IDownloaderService>().DownloadedItems.OfType<IDownloadable>().Distinct();

        /// <inheritdoc />
        public override Task Update()
        {
            return Task.Run(() =>
            {
                var allFilePaths = Directory.EnumerateFiles(FileSystem.CacheDirectory);

                var parentedFiles = from item in DownloadedItems
                            orderby item.Name
                            select item.DownloadableFile.LocalPath;

                var orphanFiles = allFilePaths.Where((string file) => !parentedFiles.Contains(file));

                foreach (var file in orphanFiles)
                {
                    File.Delete(file);
                }
            });
        }
    }
}
