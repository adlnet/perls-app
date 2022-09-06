using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.DrawerLayout.Widget;
using PERLS.Data.Definition;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// Customizes the appearance of the toolbar (the bar at the top of the screen).
    /// </summary>
    internal class StyledShellToolbarTracker : ShellToolbarTracker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyledShellToolbarTracker"/> class.
        /// </summary>
        /// <param name="shellContext">The shell context.</param>
        /// <param name="toolbar">The Android toolbar.</param>
        /// <param name="drawerLayout">The current drawer layout.</param>
        public StyledShellToolbarTracker(IShellContext shellContext, Toolbar toolbar, DrawerLayout drawerLayout) : base(shellContext, toolbar, drawerLayout)
        {
        }

        /// <inheritdoc />
        /// <remarks>The default implementation tints all icons white--but we don't want our photo to be white.</remarks>
        protected override void UpdateMenuItemIcon(Context context, IMenuItem menuItem, ToolbarItem toolBarItem)
        {
            var source = toolBarItem.IconImageSource;

            Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source)
                .LoadImageAsync(source, context)
                .ContinueWith(
                    task =>
                    {
                        if (menuItem.Icon != null && menuItem.Icon.Equals(task.Result))
                        {
                            return;
                        }

                        menuItem.SetIcon(new BitmapDrawable(context.Resources, task.Result));
                    }, TaskScheduler.Default).ConfigureAwait(false);
        }

        protected override void UpdatePageTitle(Toolbar toolbar, Page page)
        {
            toolbar.SetTitleTextColor(((Color)Application.Current.Resources[nameof(ITheme.PrimaryTextColor)]).ToAndroid());
            base.UpdatePageTitle(toolbar, page);
        }
    }
}
