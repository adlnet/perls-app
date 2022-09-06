namespace PERLS.Data.Definition
{
    /// <summary>
    /// A file that can be extracted to a local path.
    /// </summary>
    public interface IPackageFile : IFile
    {
        /// <summary>
        /// Gets the local extracted path for this packaged file.
        /// </summary>
        /// <value>The extracted local path.</value>
        string LocalExtractedPath { get; }
    }
}
