using System;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A response that may have more data which can be requested from the server.
    /// </summary>
    [Serializable]
    public abstract class PartialResponse : IPartialResponse
    {
        /// <inheritdoc />
        public Uri OriginalUri { get; set; }
    }
}
