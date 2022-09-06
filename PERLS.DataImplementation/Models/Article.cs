using System;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// An article implementation.
    /// </summary>
    [Serializable]
    public class Article : TinCanPackagedNode, IArticle
    {
        /// <summary>
        /// Gets a value indicating whether the discussion is open.
        /// </summary>
        /// <value>
        /// A value indicating whether the discussion is open.
        /// </value>
        public bool DiscussionOpen => DiscussionStatus == "open";

        [JsonProperty("discussion_status")]
        string DiscussionStatus { get; set; }
    }
}
