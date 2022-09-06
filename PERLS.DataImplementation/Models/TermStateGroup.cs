using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// Term state group.
    /// </summary>
    [Serializable]
    public class TermStateGroup
    {
        /// <summary>
        /// Gets the term states.
        /// </summary>
        /// <value>The term states.</value>
        [JsonProperty("rows")]
        public List<TermState> TermStates { get; internal set; } = new List<TermState>();
    }
}
