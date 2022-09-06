namespace PERLS.Data.Definition
{
    /// <summary>
    /// Use this to mark items as downloadable.
    /// </summary>
    public interface IDownloadable : IItem
    {
        /// <summary>
        /// Gets the downloadable file for this content.
        /// </summary>
        /// <value>The downloadable file.</value>
        IFile DownloadableFile { get; }
    }
}
