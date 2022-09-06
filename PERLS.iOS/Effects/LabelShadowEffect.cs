using System.Linq;
using PERLS.Effects;
using PERLS.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("PERLS")]
[assembly: ExportEffect(typeof(LabelShadowEffect), nameof(LabelShadowEffect))]

namespace PERLS.iOS.Effects
{
    /// <summary>
    /// Enables a shadow to be added to labels.
    /// </summary>
    /// <remarks>Inspired by the effect demo provided by Xamarin.</remarks>
    /// <see href="https://docs.microsoft.com/en-us/samples/xamarin/xamarin-forms-samples/effects-shadoweffect/"/>
    public class LabelShadowEffect : PlatformEffect
    {
        /// <summary>
        /// Gets the current effect instance on the element.
        /// </summary>
        ShadowEffect Effect => Element?.Effects.FirstOrDefault(e => e is ShadowEffect) as ShadowEffect;

        /// <inheritdoc />
        protected override void OnAttached()
        {
            if (Effect?.IsVisible != true)
            {
                return;
            }

            Control.Layer.CornerRadius = Effect.Radius;
            Control.Layer.ShadowColor = Effect.Color.ToCGColor();
            Control.Layer.ShadowOffset = new CoreGraphics.CGSize(Effect.DistanceX, Effect.DistanceY);
            Control.Layer.ShadowOpacity = 1.0f;
        }

        /// <inheritdoc />
        protected override void OnDetached()
        {
            if (Control != null && Control.Layer != null)
            {
                Control.Layer.ShadowOpacity = 0f;
            }
        }
    }
}
