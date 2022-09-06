using System;
using System.ComponentModel;
using Newtonsoft.Json;
using PERLS.Data.Definition;
using PERLS.Data.ExperienceAPI;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The badge implementation.
    /// </summary>
    [Serializable]
    public class Badge : Node, IBadge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Badge"/> class.
        /// </summary>
        public Badge()
        {
        }

        /// <inheritdoc />
        [JsonProperty("id")]
        public string ID { get; protected set; }

        /// <inheritdoc />
        [JsonProperty("uuid")]
        public string UUID { get; protected set; }

        /// <inheritdoc />
        [JsonProperty("label")]
        public string Label { get; protected set; }

        /// <inheritdoc />
        [JsonProperty("plugin_type")]
        public string BadgeType { get; protected set; }

        /// <inheritdoc />
        [JsonProperty("secret")]
        public bool IsSecret { get; protected set; }

        /// <inheritdoc />
        [JsonProperty("invisible")]
        public bool IsInvisible { get; protected set; }

        /// <inheritdoc />
        [JsonProperty("unlocked_image_url")]
        public Uri UnlockedImageUri { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("locked_image_url")]
        public Uri LockedImageUri { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("unlocked")]
        public bool IsUnlocked { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("unlocked_timestamp")]
        [JsonConverter(typeof(PERLS.Data.DateTimeOffsetConverter))]
        public DateTimeOffset? LastEarned { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("description")]
        public new string Description { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("status_description")]
        public string StatusDescription { get; protected set; }

        /// <inheritdoc/>
        public Uri TinCanActivityId => ActivityBuilder.FromBadge(this).id;

        /// <inheritdoc/>
        public Uri TinCanActivityType => new Uri("http://activitystrea.ms/schema/1.0/badge");
    }
}
