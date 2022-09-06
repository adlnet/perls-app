using System;
using System.ComponentModel;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// An annotation interface.
    /// </summary>
    public interface IAnnotation : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the Date the annotation was created.
        /// </summary>
        /// <value>
        /// The Date the annotation was created.
        /// </value>
        DateTimeOffset DateCreated { get; }

        /// <summary>
        /// Gets the text that was highlighted.
        /// </summary>
        /// <value>
        /// The text that was highlighted.
        /// </value>
        string HighlightedText { get; }

        /// <summary>
        /// Gets the note accompanying the annotation.
        /// </summary>
        /// <value>
        /// The note accompanying the annotation.
        /// </value>
        string UserNote { get; }

        /// <summary>
        /// Gets the URI of the article node as a string.
        /// </summary>
        /// <value>
        /// The URI of the article node.
        /// </value>
        Uri NodeUri { get; }

        /// <summary>
        /// Gets the title of the article node.
        /// </summary>
        /// <value>
        /// The title of the article node.
        /// </value>
        string NodeTitle { get; }

        /// <summary>
        /// Gets the id of the statement as a string.
        /// </summary>
        /// <value>
        /// The id of the statement as a string.
        /// </value>
        string StatementId { get; }
    }
}
