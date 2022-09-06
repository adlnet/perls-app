using System;
using System.IO;
using Float.TinCan.ActivityLibrary;
using PERLS.Data.Definition;
using Xamarin.Essentials;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class File : DrupalEntity, IFile
    {
        /// <inheritdoc />
        public string LocalPath => Path.Combine(FileStorage.ApplicationDataDirectory, Name);

        /// <inheritdoc />
        public bool IsDownloaded => System.IO.File.Exists(LocalPath);

        /// <inheritdoc />
        public string ETag { get; set; }
    }
}
