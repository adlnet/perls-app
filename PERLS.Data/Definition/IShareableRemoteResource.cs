using System;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Interface for content sharing.
    /// </summary>
    public interface IShareableRemoteResource : IRemoteResource
    {
        /// <summary>
        /// Gets a value indicating whether a content type is shareable.
        /// </summary>
        /// <value>Whether a content type is shareable.</value>
        bool CanBeShared { get; }

        /// <summary>
        /// Gets the uri of the shareable content.
        /// </summary>
        /// <value>The uri of the shareable content.</value>
        Uri ShareableUri { get; }

        /// <summary>
        /// Gets the description of the shareable content.
        /// </summary>
        /// <value>The description of the shareable content.</value>
        string ShareableDescription { get; }
    }
}
