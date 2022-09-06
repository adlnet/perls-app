using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class Course : Node, ICourse
    {
        /// <summary>
        /// Gets the learning objects.
        /// </summary>
        /// <value>The learning objects.</value>
        [JsonProperty("learning_objects")]
        public IList<Node> LearningObjects { get; internal set; }

        /// <inheritdoc />
        IEnumerable<IItem> ICourse.LearningObjects => LearningObjects;
    }
}
