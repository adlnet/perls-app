using PERLS.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Frame), typeof(DefaultFrameRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// The default frame renderer.  Used to adjust drop shadow.
    /// </summary>
    public class DefaultFrameRenderer : FrameRenderer
    {
        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                if (Element.HasShadow)
                {
                    Layer.ShadowOpacity = .25f;
                    Layer.ShadowOffset = new CoreGraphics.CGSize(0, 3);
                    Layer.ShadowRadius = (System.nfloat)1.0;
                }
            }
        }
    }
}
