using System.ComponentModel;
using Android.Content;
using PERLS.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Button), typeof(DefaultButtonRenderer))]

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// Default button renderer.
    /// </summary>
    public class DefaultButtonRenderer : Xamarin.Forms.Platform.Android.FastRenderers.ButtonRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultButtonRenderer"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public DefaultButtonRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            Control.Text = Element.Text;
        }
    }
}
