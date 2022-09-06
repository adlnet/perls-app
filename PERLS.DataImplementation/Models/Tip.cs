using System;
using System.Collections.Generic;
using PERLS.Data.Definition;
using PERLS.Data.ParagraphLayout.Models;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class Tip : Node, ITip
    {
        /// <summary>
        /// Gets the contents.
        /// </summary>
        /// <value>The contents.</value>
        public IList<string> Contents { get; internal set; }

        /// <inheritdoc />
        public IList<Paragraph> Body { get; internal set; }

        /// <inheritdoc />
        public new string Description => Contents != null ? string.Join("\n", Contents) : null;
    }
}
