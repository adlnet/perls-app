using System;
using System.IO;
using Float.TinCan.ActivityLibrary;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class PackageFile : IFile, IPackageFile
    {
        internal const string ZipExtension = ".ZIP";

        readonly IFile originalFile;

        internal PackageFile(File file)
        {
            this.originalFile = file;
        }

        /// <inheritdoc />
        public string LocalExtractedPath => Path.Combine(FileStorage.PackagedContentDirectory, Path.GetFileNameWithoutExtension(originalFile.Name));

        /// <inheritdoc />
        public bool IsDownloaded => Directory.Exists(LocalExtractedPath);

        /// <inheritdoc />
        public string Name => originalFile.Name;

        /// <inheritdoc />
        public string LocalPath => originalFile.LocalPath;

        /// <inheritdoc />
        public Uri Url => originalFile.Url;

        /// <inheritdoc />
        public string ETag
        {
            get => originalFile.ETag;
            set => originalFile.ETag = value;
        }
    }
}
