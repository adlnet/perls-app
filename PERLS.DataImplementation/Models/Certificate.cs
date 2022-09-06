using System;
using System.ComponentModel;
using Newtonsoft.Json;
using PERLS.Data;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The certificate implementation.
    /// </summary>
    [Serializable]
    public class Certificate : Node, ICertificate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Certificate"/> class.
        /// </summary>
        public Certificate()
        {
        }

        /// <inheritdoc/>
        [JsonProperty("id")]
        public string ID { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("uuid")]
        public string UUID { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("label")]
        public string CertificateName { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("unlocked_timestamp")]
        [JsonConverter(typeof(PERLS.Data.DateTimeOffsetConverter))]
        public DateTimeOffset ReceivedTime { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("unlocked_image_url")]
        public Uri ThumbnailImageUri { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("sharable_image_url")]
        public Uri ShareableImageUri { get; protected set; }
    }
}
