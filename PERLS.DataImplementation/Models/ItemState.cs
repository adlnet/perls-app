using System;
using Newtonsoft.Json;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// Item state.
    /// </summary>
    [Serializable]
    public class ItemState : BaseState
    {
        /// <summary>
        /// Gets nid.
        /// </summary>
        /// <value>The nid.</value>
        public int Nid { get; internal set; }

        /// <summary>
        /// Gets recommendation reason.
        /// </summary>
        /// <value>The recommendation reason.</value>
        [JsonProperty("Reason")]
        public string RecommendationReason { get; internal set; }

        /// <summary>
        /// Gets the learner's attempts on this item.
        /// </summary>
        /// <value>The test attempts.</value>
        [JsonProperty("result")]
        public TestAttempt LatestAttempt { get; internal set; }
    }
}
