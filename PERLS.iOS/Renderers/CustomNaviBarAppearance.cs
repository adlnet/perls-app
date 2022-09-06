using System.Linq;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Shell), typeof(PERLS.iOS.Renderers.DefaultShellRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Custom navi bar appearance.
    /// </summary>
    public class CustomNaviBarAppearance : IShellNavBarAppearanceTracker
    {
        UIColor defaultTint;
        UIStringAttributes defaultTitleAttributes;

        /// <inheritdoc/>
        public void ResetAppearance(UINavigationController controller)
        {
            if (defaultTint != null && controller != null)
            {
                var navBar = controller.NavigationBar;
                navBar.TintColor = defaultTint;
                navBar.TitleTextAttributes = defaultTitleAttributes;
                navBar.Translucent = false;
            }
        }

        /// <inheritdoc/>
        public void SetAppearance(UINavigationController controller, ShellAppearance appearance)
        {
            if (appearance == null)
            {
                return;
            }

            var background = appearance.BackgroundColor;
            var foreground = appearance.ForegroundColor;
            var titleColor = (Color)Xamarin.Forms.Application.Current.Resources[nameof(ITheme.PrimaryTextColor)];
            defaultTitleAttributes = new UIStringAttributes
            {
                Font = UIFont.FromName("Open Sans-Bold", 23f),
                ForegroundColor = titleColor.ToUIColor(),
            };

            if (controller == null)
            {
                return;
            }

            var navBar = controller.NavigationBar;

            if (defaultTint == null)
            {
                defaultTint = navBar.TintColor;
            }

            if (!background.IsDefault)
            {
                navBar.BarTintColor = background.ToUIColor();
            }

            if (!foreground.IsDefault)
            {
                navBar.TintColor = foreground.ToUIColor();
            }

            navBar.TitleTextAttributes = defaultTitleAttributes;
        }

        /// <inheritdoc/>
        public void UpdateLayout(UINavigationController controller)
        {
            var navBar = controller?.NavigationBar;
            if (navBar == null || navBar.Items == null)
            {
                return;
            }

            navBar.Items.SelectMany((item) => item.RightBarButtonItems).ForEach((item) =>
            {
                item.TintColor = ((Color)Xamarin.Forms.Application.Current.Resources["SecondaryColor"]).ToUIColor();
            });
        }

        /// <inheritdoc/>
#pragma warning disable CA1816, CA1063
        public void Dispose()
#pragma warning restore CA1816, CA1063
        {
        }

        /// <inheritdoc/>
        public void SetHasShadow(UINavigationController controller, bool hasShadow)
        {
        }

        /// <summary>
        /// Dispose the specified disposing.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
