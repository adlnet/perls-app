using Android.Content;
using AndroidX.AppCompat.Widget;
using PERLS.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Shell), typeof(StyledShellRenderer))]

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// Customizes the appearance and behavior of the PERLS shell on Android.
    /// </summary>
    public class StyledShellRenderer : ShellRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyledShellRenderer"/> class.
        /// </summary>
        /// <param name="context">Current Android context.</param>
        public StyledShellRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc />
        protected override IShellToolbarTracker CreateTrackerForToolbar(Toolbar toolbar)
        {
            return new StyledShellToolbarTracker(this, toolbar, ((IShellContext)this).CurrentDrawerLayout);
        }

        /// <inheritdoc />
        protected override IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker()
        {
            var tracker = new StyledShellToolbarAppearanceTracker(this);
            return tracker;
        }
    }
}
