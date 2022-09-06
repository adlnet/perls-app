using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Shell), typeof(PERLS.iOS.Renderers.DefaultShellRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Default shell renderer.
    /// </summary>
    public class DefaultShellRenderer : ShellRenderer
    {
        /// <inheritdoc/>
        protected override IShellNavBarAppearanceTracker CreateNavBarAppearanceTracker()
        {
            return new CustomNaviBarAppearance();
        }

        /// <inheritdoc/>
        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {
            return new PERLSShellSectionRenderer(this);
        }

        /// <inheritdoc/>
        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem item)
        {
            var renderer = base.CreateShellItemRenderer(item);
            (renderer as ShellItemRenderer).TabBar.Translucent = false;
            return renderer;
        }
    }
}
