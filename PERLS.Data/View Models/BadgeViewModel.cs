using System;
using FFImageLoading.Svg.Forms;
using Float.Core.ViewModels;
using Humanizer;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The Badge View Model.
    /// </summary>
    public class BadgeViewModel : SelectableViewModel<IBadge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadgeViewModel"/> class.
        /// </summary>
        /// <param name="badge">The badge.</param>
        public BadgeViewModel(IBadge badge) : base(badge)
        {
            Model = badge ?? throw new ArgumentNullException(nameof(badge));
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public new IBadge Model { get; protected set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>The Label.</value>
        public string Label => Model.Label;

        /// <summary>
        /// Gets the label for the detail screen.
        /// </summary>
        /// <value>The label.</value>
        public string DetailLabel => Model.Label;

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The Description of the badge.</value>
        public string Description => Model.Description;

        /// <summary>
        /// Gets a value indicating whether this badge should be displayed.
        /// </summary>
        /// <remarks>
        /// Invisible badges should never be shown.
        /// Secret badges should only be shown if already earned.
        /// </remarks>
        /// <value>True if visible, False if not.</value>
        public bool IsVisible => !Model.IsInvisible && (!Model.IsSecret || Model.IsUnlocked);

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>The image.</value>
        [NotifyWhenPropertyChanges(nameof(IBadge.IsUnlocked))]
        [NotifyWhenPropertyChanges(nameof(IBadge.UnlockedImageUri))]
        [NotifyWhenPropertyChanges(nameof(IBadge.LockedImageUri))]
        public Uri ImageUri => Model.IsUnlocked ? Model.UnlockedImageUri : Model.LockedImageUri;

        /// <summary>
        /// Gets a placeholder image for a badge.
        /// </summary>
        /// <value>A placeholder image.</value>
        public ImageSource Placeholder
        {
            get
            {
                return SvgImageSource.FromResource("PERLS.Data.Resources.badge_placeholder.svg");
            }
        }

        /// <summary>
        /// Gets when this item was earned.
        /// </summary>
        /// <value>A human friendly time ago.</value>
        public string LastEarned => Model.LastEarned.Humanize().Transform(To.TitleCase);

        /// <summary>
        /// Gets a value indicating whether this is unlocked.
        /// </summary>
        /// <value>Has this badge been unlocked.</value>
        public bool IsUnlocked => Model.IsUnlocked;

        /// <summary>
        /// Gets when this badge was earned.
        /// </summary>
        /// <value>
        /// When this badge was earned.
        /// </value>
        [NotifyWhenPropertyChanges(nameof(IBadge.LastEarned))]
        [NotifyWhenPropertyChanges(nameof(IBadge.IsUnlocked))]
        public string EarnedOn => Model.IsUnlocked ? $"{Model.StatusDescription} {Model.LastEarned.Humanize()}" : Model.StatusDescription;
    }
}
