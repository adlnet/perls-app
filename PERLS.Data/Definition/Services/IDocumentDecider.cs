using System;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// Determines if a Uri should be opened via Document opener.
    /// </summary>
    public interface IDocumentDecider
    {
        /// <summary>
        /// Checks if the filepath is an openable document.
        /// </summary>
        /// <param name="uri">The uri containing the filepath to be checked.</param>
        /// <returns><c>True</c> if the Uri is a document to be opened.</returns>
        bool IsOpenableDocument(Uri uri);

        /// <summary>
        /// Checks if URL should be opened via document opener.
        /// </summary>
        /// <param name="uri">The URI of the request.</param>
        /// <returns>The filepath of the document if the file exists or null.</returns>
        string GetDocumentFilePath(Uri uri);
    }
}
