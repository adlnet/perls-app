using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class Podcast : Node, IPodcast
    {
        /// <inheritdoc />
        public IEnumerable<IEpisode> Episodes { get; set; }

        /// <inheritdoc />
        [JsonProperty("number_episodes")]
        public int EpisodesCount { get; set; }
    }
}
