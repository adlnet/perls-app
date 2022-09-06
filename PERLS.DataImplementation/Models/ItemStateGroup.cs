using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// Item state group.
    /// </summary>
    [Serializable]
    public class ItemStateGroup
    {
        /// <summary>
        /// Gets the item states.
        /// </summary>
        /// <value>The item states.</value>
        [JsonProperty("rows")]
        public List<ItemState> ItemStates { get; internal set; } = new List<ItemState>();
    }
}
