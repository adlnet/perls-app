using Float.TinCan.ActivityLibrary.Definition;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Represents any content with a packaged representation that can be locally served.
    /// </summary>
    public interface IPackagedContent : IItem, IActivity, IDownloadable
    {
        /// <summary>
        /// Gets the file for downloading this article's archived representation.
        /// </summary>
        /// <value>The archived article.</value>
        IPackageFile PackageFile { get; }
    }
}
