using System;
using System.Collections.Generic;

namespace PERLS.Data.ParagraphLayout.Models
{
    /// <summary>
    /// A individual field.
    /// </summary>
    [Serializable]
    public class Field
    {
        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>The field type.</value>
        public string Type { get; internal set; }

        /// <summary>
        /// Gets the attributes of the field (i.e. the text of the label or source of the image).
        /// </summary>
        /// <value>The field attributes.</value>
        public Dictionary<string, string> Attributes { get; internal set; }
    }
}
