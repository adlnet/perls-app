using System;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A node that also includes packaged content.
    /// </summary>
    [Serializable]
    public abstract class TinCanPackagedNode : TinCanNode, IPackagedContent
    {
        /// <inheritdoc />
        IPackageFile IPackagedContent.PackageFile =>
            PackageFile?.Name?.EndsWith(Models.PackageFile.ZipExtension, StringComparison.InvariantCultureIgnoreCase) == true
            ? new PackageFile(PackageFile)
            : null;

        /// <inheritdoc />
        public IFile DownloadableFile => (this as IPackagedContent).PackageFile;
    }
}
