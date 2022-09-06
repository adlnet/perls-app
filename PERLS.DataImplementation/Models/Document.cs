using System;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A Document implementation.
    /// </summary>
    [Serializable]
    public class Document : Node, IDocument
    {
        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <value>The file.</value>
        public File File { get; internal set; }

        /// <inheritdoc />
        public IFile DownloadableFile => File;

        /// <inheritdoc />
        IFile IDocument.File => File;
    }
}
