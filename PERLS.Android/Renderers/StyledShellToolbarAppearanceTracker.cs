using System;
using AndroidX.AppCompat.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// Customizes the appearance and behavior of the PERLS shell toolbar on Android.
    /// </summary>
    public class StyledShellToolbarAppearanceTracker : ShellToolbarAppearanceTracker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyledShellToolbarAppearanceTracker"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public StyledShellToolbarAppearanceTracker(IShellContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public override void SetAppearance(Toolbar toolbar, IShellToolbarTracker toolbarTracker, ShellAppearance appearance)
        {
            base.SetAppearance(toolbar, toolbarTracker, appearance);
            if (toolbarTracker == null)
            {
                return;
            }

            toolbarTracker.TintColor = (Color)Application.Current.Resources["SecondaryColor"];
            if (toolbar != null && toolbar.NavigationIcon != null)
            {
                toolbar.NavigationIcon.ClearColorFilter();
            }
        }
    }
}
