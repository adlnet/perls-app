using System;
using System.ComponentModel;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The certificate interface.
    /// </summary>
    public interface ICertificate : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        string ID { get; }

        /// <summary>
        /// Gets the UUID.
        /// </summary>
        /// <value>
        /// The UUID.
        /// </value>
        string UUID { get; }

        /// <summary>
        /// Gets the certificate name.
        /// </summary>
        /// <value>
        /// The certificate name.
        /// </value>
        string CertificateName { get; }

        /// <summary>
        /// Gets the time in which they received it, most likely the time in which they most recently completed an activity.
        /// </summary>
        /// <value>
        /// The time in which they received it, most likely the time in which they most recently completed an activity.
        /// </value>
        DateTimeOffset ReceivedTime { get; }

        /// <summary>
        /// Gets the thumbnail Uri.
        /// </summary>
        /// <value>
        /// The thumbnail Uri.
        /// </value>
        Uri ThumbnailImageUri { get; }

        /// <summary>
        /// Gets the shareable image uri.
        /// </summary>
        /// <value>
        /// The shareable image uri.
        /// </value>
        Uri ShareableImageUri { get; }
    }
}
