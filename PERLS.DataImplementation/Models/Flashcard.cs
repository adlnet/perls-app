using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PERLS.Data.Definition;
using PERLS.Data.ParagraphLayout.Models;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// Flashcard.
    /// </summary>
    [Serializable]
    public class Flashcard : Node, IFlashcard
    {
        /// <summary>
        /// Gets the contents.
        /// </summary>
        /// <value>The contents.</value>
        public IList<string> Contents { get; internal set; }

        /// <inheritdoc />
        public new string Description => Contents != null ? string.Join("\n", Contents) : null;

        /// <inheritdoc />
        public string BackContent => BackOfCard != null ? string.Join("\n", BackOfCard) : null;

        /// <summary>
        /// Gets back of card.
        /// </summary>
        /// <value>The back of card.</value>
        [JsonProperty("back_contents")]
        public IList<string> BackOfCard { get; internal set; }

        /// <inheritdoc />
        public IList<Paragraph> Body { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("body_back")]
        public IList<Paragraph> BackBody { get; internal set; }
    }
}
