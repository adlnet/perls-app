using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Types of templates that could be used to represent block contents.
    /// </summary>
    public enum BlockTemplate
    {
        /// <summary>
        /// Block contents should be presented in small text-only tiles.
        /// </summary>
        Chip,

        /// <summary>
        /// Block contents should be presented in teasers.
        /// </summary>
        Tile,

        /// <summary>
        /// Block contents should be presented in cards.
        /// </summary>
        Card,

        /// <summary>
        /// Block contents should be presented in prompts.
        /// </summary>
        [EnumMember(Value = "simple_banner")]
        Banner,
    }
}
