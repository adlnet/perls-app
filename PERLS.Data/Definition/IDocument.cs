namespace PERLS.Data.Definition
{
    /// <summary>
    /// A document item, representing an item with a file.
    /// </summary>
    public interface IDocument : IItem, IDownloadable
    {
        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <value>The file.</value>
        IFile File { get; }
    }
}
