using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Types of entities that could be represented in this block.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityType
    {
        /// <summary>
        /// Node entities.
        /// </summary>
        Node,

        /// <summary>
        /// Taxonomy term entities.
        /// </summary>
        [EnumMember(Value = "taxonomy_term")]
        TaxonomyTerm,

        /// <summary>
        /// Group entities.
        /// </summary>
        Group,
    }
}
