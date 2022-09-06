using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Defines potential layouts for a section.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SectionLayout
    {
        /// <summary>
        /// This section should be presented in a single column of data.
        /// </summary>
        [EnumMember(Value = "layout_onecol")]
        OneColumn,

        /// <summary>
        /// This section should be presented with a tab for each block.
        /// </summary>
        [EnumMember(Value = "tabs")]
        Tabs,
    }
}
