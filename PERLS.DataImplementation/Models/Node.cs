using System;
using System.Collections.Generic;
using System.ComponentModel;
using JsonSubTypes;
using Newtonsoft.Json;
using PERLS.Data.Definition;
using PERLS.Data.ExperienceAPI;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A Drupal node.
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(JsonSubtypes), nameof(Type))]
    [JsonSubtypes.KnownSubType(typeof(Tip), "tip_card")]
    [JsonSubtypes.KnownSubType(typeof(Quiz), "quiz")]
    [JsonSubtypes.KnownSubType(typeof(Course), "course")]
    [JsonSubtypes.KnownSubType(typeof(Flashcard), "flash_card")]
    [JsonSubtypes.KnownSubType(typeof(Document), "learn_file")]
    [JsonSubtypes.KnownSubType(typeof(Article), "learn_article")]
    [JsonSubtypes.KnownSubType(typeof(Package), "learn_package")]
    [JsonSubtypes.KnownSubType(typeof(Podcast), "podcast")]
    [JsonSubtypes.KnownSubType(typeof(Episode), "podcast_episode")]
    [JsonSubtypes.KnownSubType(typeof(Test), "test")]
    [JsonSubtypes.KnownSubType(typeof(Link), "learn_link")]
    public class Node : DrupalEntity, IItem, IExperienceAPIActivity, IShareableRemoteResource
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged
        {
            // This event is not currently supported...
            // but required to be here because of Float.Core.
            add { }
            remove { }
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>The image.</value>
        public File Image { get; internal set; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public IList<Tag> Tags { get; internal set; } = new List<Tag>();

        /// <summary>
        /// Gets the topic.
        /// </summary>
        /// <value>The topic.</value>
        public Topic Topic { get; internal set; }

        /// <inheritdoc />
        public string Description { get; internal set; }

        /// <summary>
        /// Gets the nid.
        /// </summary>
        /// <value>The nid.</value>
        public int Nid { get; internal set; }

        /// <inheritdoc />
        IEnumerable<ITag> IItem.Tags => Tags;

        /// <inheritdoc />
        ITopic IItem.Topic => Topic;

        /// <inheritdoc />
        IFile IItem.Image => Image;

        /// <inheritdoc />
        Uri IExperienceAPIActivity.Id => new Uri($"node/{Nid}", UriKind.Relative);

        /// <inheritdoc />
        public bool CanBeShared => true;

        /// <inheritdoc />
        public Uri ShareableUri => new Uri(AppConfig.Server, Url);

        /// <inheritdoc />
        public string ShareableDescription => Description;
    }
}
