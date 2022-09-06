using System;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Interface for a partial response.
    /// </summary>
    public interface IPartialResponse
    {
        /// <summary>
        /// Gets or sets the original uri of a partial response. Useful reference if the uri is ever modified.
        /// </summary>
        /// <value>Returns the original uri of a partial response.</value>
        Uri OriginalUri { get; set; }
    }
}
