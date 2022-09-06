using System;
using Float.TinCan.ActivityLibrary.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A class to implement <see cref="IActivityMetaData"/>.
    /// </summary>
    [Serializable]
    public class TinCanMetaData : IActivityMetaData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TinCanMetaData"/> class.
        /// </summary>
        /// <param name="start">The starting URL of the activity.</param>
        /// <param name="id">The unique ID of the activity.</param>
        /// <param name="title">The title of the activity.</param>
        /// <param name="time">The last modification time of the activity.</param>
        public TinCanMetaData(string start, string id, string title, DateTimeOffset? time = null)
        {
            this.StartLocation = start;
            this.UUID = id;
            this.Title = title;
            this.LastModificationTime = time ?? DateTimeOffset.Now;
        }

        /// <inheritdoc />
        public string StartLocation { get; }

        /// <inheritdoc />
        public string UUID { get; }

        /// <inheritdoc />
        public string Title { get; }

        /// <inheritdoc />
        public DateTimeOffset LastModificationTime { get; }
    }
}
