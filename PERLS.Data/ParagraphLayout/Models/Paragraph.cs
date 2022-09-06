using System;
using System.Collections.Generic;

namespace PERLS.Data.ParagraphLayout.Models
{
    /// <summary>
    /// A group of fields.
    /// </summary>
    [Serializable]
    public class Paragraph
    {
        /// <summary>
        /// Gets the id of the paragraph.
        /// </summary>
        /// <value>The paragraph ID.</value>
        public Guid Id { get; internal set; }

        /// <summary>
        /// Gets the type of the paragraph.
        /// </summary>
        /// <value>The paragraph type.</value>
        public string Type { get; internal set; }

        /// <summary>
        /// Gets the fields contained by the paragraph.
        /// </summary>
        /// <value>The paragraph fields.</value>
        public IList<Field> Fields { get; internal set; }
    }
}
