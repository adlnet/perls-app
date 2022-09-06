using System;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Represents a generic resource that is remote.
    /// </summary>
    public interface IRemoteResource : INamedNotifyPropertyChanged
    {
        /// <summary>
        /// Gets the unique ID for the resource.
        /// </summary>
        /// <value>A unique resource ID.</value>
        Guid Id { get; }

        /// <summary>
        /// Gets the URL for the resource.
        /// </summary>
        /// <value>The resource URL.</value>
        Uri Url { get; }
    }
}
