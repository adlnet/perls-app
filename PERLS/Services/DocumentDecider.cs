using System;
using System.Collections.Specialized;
using System.IO;
using Float.HttpServer;
using Float.TinCan.ActivityLibrary;
using PERLS.Data.Definition.Services;

namespace PERLS.Services
{
    /// <summary>
    /// Determines if a Uri should be opened via Document opener.
    /// </summary>
    public class DocumentDecider : IDocumentDecider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDecider"/> class.
        /// </summary>
        public DocumentDecider()
        {
        }

        /// <summary>
        /// Checks if the filepath is an openable document.
        /// </summary>
        /// <param name="uri">The uri containing the filepath to be checked.</param>
        /// <returns><c>True</c> if the Uri is a document to be opened.</returns>
        public bool IsOpenableDocument(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var filepath = uri.OriginalString;

            if (uri.Scheme != "file" || !File.Exists(filepath))
            {
                return false;
            }

            var allowedDocumentExtensions = new StringCollection
            {
                ".pdf",
                ".docx",
                ".doc",
                ".pptx",
                ".ppt",
                ".xlsx",
                ".xls",
                ".txt",
                ".csv",
            };

            return allowedDocumentExtensions.Contains(Path.GetExtension(filepath));
        }

        /// <summary>
        /// Checks if URL should be opened via document opener.
        /// </summary>
        /// <param name="uri">The URI of the request.</param>
        /// <returns>The filepath of the document if the file exists or null.</returns>
        public string GetDocumentFilePath(Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            var filepath = StaticFileResponder.FilePathForRequest(FileStorage.PackagedContentDirectory, uri);
            return IsOpenableDocument(new Uri(filepath)) ? filepath : null;
        }
    }
}
