using System;
using System.ComponentModel;
using Float.TinCan.ActivityLibrary.Definition;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The Badge interface.
    /// </summary>
    public interface IBadge : INotifyPropertyChanged, ITinCanActivity
    {
        /// <summary>
        /// Gets the Badge ID.
        /// </summary>
        /// <value>The Badge ID.</value>
        string ID { get; }

        /// <summary>
        /// Gets the UUID.
        /// </summary>
        /// <value>The UUID.</value>
        string UUID { get; }

        /// <summary>
        /// Gets the Badge label.
        /// </summary>
        /// <value>The label of the badge (i.e. Badge Name).</value>
        string Label { get; }

        /// <summary>
        /// Gets the Badge Description.
        /// </summary>
        /// <value>The description of the badge.</value>
        string Description { get; }

        /// <summary>
        /// Gets the Badge Type.
        /// </summary>
        /// <value>The type of badge (e.g. Touchpoint, time streak, streak).</value>
        string BadgeType { get; }

        /// <summary>
        /// Gets a value indicating whether badge is secret.
        /// </summary>
        /// <value> A boolean to say if badge should be show if not awarded. Secret badges remain hidden until unlocked.</value>
        bool IsSecret { get; }

        /// <summary>
        /// Gets a value indicating whether badge is invisible.
        /// </summary>
        /// <value> This type of badge will never been shown to the user.</value>
        bool IsInvisible { get; }

        /// <summary>
        /// Gets the Unlocked Image Uri.
        /// </summary>
        /// <value>The Uri to a thumbnail image for the badge when unlocked.</value>
        Uri UnlockedImageUri { get; }

        /// <summary>
        /// Gets the locked Image Uri.
        /// </summary>
        /// <value>The Uri to a thumbnail image for the badge when locked.</value>
        Uri LockedImageUri { get; }

        /// <summary>
        /// Gets a value indicating whether badge is unlocked.
        /// </summary>
        /// <value> True is the user has unlocked this badge.</value>
        bool IsUnlocked { get; }

        /// <summary>
        /// Gets the timestamp this badge was last earned.
        /// </summary>
        /// <value>The date/time for when the item was earned or null if not earned.</value>
        DateTimeOffset? LastEarned { get; }

        /// <summary>
        /// Gets a text description of the current status of badge.
        /// </summary>
        /// <value>The status of the badge.</value>
        string StatusDescription { get; }
    }
}
