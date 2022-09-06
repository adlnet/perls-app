using PERLS.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(DefaultButtonRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Overrides the default button.
    /// </summary>
    public class DefaultButtonRenderer : ButtonRenderer
    {
        /// <inheritdoc />
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.TitleLabel.Lines = 0;
                Control.TitleLabel.TextAlignment = UIKit.UITextAlignment.Center;
            }
        }
    }
}
