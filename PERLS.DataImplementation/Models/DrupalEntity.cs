using System;
using System.Collections.Generic;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A Drupal entity.
    /// </summary>
    [Serializable]
    public abstract class DrupalEntity : IEquatable<DrupalEntity>
    {
        /// <summary>
        /// Gets the UUID.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; internal set; }

        /// <summary>
        /// Gets the revision ID.
        /// </summary>
        /// <value>The revision ID.</value>
        public int Vid { get; internal set; }

        /// <summary>
        /// Gets the bundle type of this entity.
        /// </summary>
        /// <value>The Drupal bundle ID.</value>
        public string Type { get; internal set; }

        /// <summary>
        /// Gets the timestamp this entity was last changed.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime Changed { get; internal set; }

        /// <summary>
        /// Gets the name/label of the entity.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or Sets the local URL.
        /// </summary>
        /// <value>The local URL.</value>
        public Uri Url
        {
            get => LocalUrl;
            set => LocalUrl = value;
        }

        /// <summary>
        /// Gets or Sets the Current URL.
        /// </summary>
        /// <value>The local URL.</value>
        Uri LocalUrl { get; set; }

        /// <summary>
        /// Determine if two <see cref="DrupalEntity"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if both instances are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator ==(DrupalEntity left, DrupalEntity right)
        {
            return EqualityComparer<DrupalEntity>.Default.Equals(left, right);
        }

        /// <summary>
        /// Determine if two <see cref="DrupalEntity"/> are inequal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if both instances are not equivalent, <c>false</c> otherwise.</returns>
        public static bool operator !=(DrupalEntity left, DrupalEntity right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as DrupalEntity);
        }

        /// <inheritdoc />
        public bool Equals(DrupalEntity other)
        {
            return other != null &&
                   Id.Equals(other.Id);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name} ({Id}; {base.ToString()})";
        }
    }
}
