using Android.Content;
using PERLS.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Frame), typeof(DefaultFrameRenderer))]

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// The default frame renderer.  Used to adjust drop shadow.
    /// </summary>
    public class DefaultFrameRenderer : Xamarin.Forms.Platform.Android.FastRenderers.FrameRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFrameRenderer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public DefaultFrameRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (Element.HasShadow)
            {
                CardElevation = 7;
            }
        }
    }
}
