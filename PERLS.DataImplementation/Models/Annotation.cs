using System;
using System.ComponentModel;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The Annotation implementation.
    /// </summary>
    public class Annotation : IAnnotation
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        [JsonProperty("date")]
        public DateTimeOffset DateCreated { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("quote")]
        public string HighlightedText { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("text")]
        public string UserNote { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("node_url")]
        public Uri NodeUri { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("node_title")]
        public string NodeTitle { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("statement_id")]
        public string StatementId { get; protected set; }
    }
}
