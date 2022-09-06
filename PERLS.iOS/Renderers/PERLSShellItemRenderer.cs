using Xamarin.Forms.Platform.iOS;

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// The renderer for a ShellItem.
    /// </summary>
    public class PERLSShellItemRenderer : ShellItemRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PERLSShellItemRenderer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PERLSShellItemRenderer(IShellContext context) : base(context)
        {
        }
    }
}
