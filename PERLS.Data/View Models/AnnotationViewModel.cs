using System;
using Float.Core.ViewModels;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The view model.
    /// </summary>
    public class AnnotationViewModel : ViewModel<IAnnotation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationViewModel"/> class.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public AnnotationViewModel(IAnnotation annotation) : base(annotation)
        {
        }

        /// <summary>
        /// Gets the title of the note.
        /// </summary>
        /// <value>
        /// The title of the note.
        /// </value>
        public string NoteTitle => Model.UserNote;

        /// <summary>
        /// Gets the article title.
        /// </summary>
        /// <value>
        /// The article title.
        /// </value>
        public string ArticleTitle => Model.NodeTitle;

        /// <summary>
        /// Gets the date on which the note was made.
        /// </summary>
        /// <value>
        /// The date on which the note was made.
        /// </value>
        public DateTimeOffset DateCreated => Model.DateCreated;

        /// <summary>
        /// Gets the date on which the note was made.
        /// </summary>
        /// <value>
        /// The string formatted date on which the note was made.
        /// </value>
        public string NoteDate => Model.DateCreated.ToString("MMMM d, yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo);

        /// <summary>
        /// Gets the node/Article URI.
        /// </summary>
        /// <value>
        /// The node/Article URI.
        /// </value>
        public Uri NodeUri => Model.NodeUri;
    }
}
