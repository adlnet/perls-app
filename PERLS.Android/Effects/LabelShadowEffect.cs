using System.Linq;
using PERLS.Droid.Effects;
using PERLS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("PERLS")]
[assembly: ExportEffect(typeof(LabelShadowEffect), nameof(LabelShadowEffect))]

namespace PERLS.Droid.Effects
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
            if (Control is not Android.Widget.TextView textView || Effect?.IsVisible != true)
            {
                return;
            }

            var densityAdjustedRadius = Effect.Radius * Control.Context.Resources.DisplayMetrics.Density;
            textView.SetShadowLayer(densityAdjustedRadius, Effect.DistanceX, Effect.DistanceY, Effect.Color.ToAndroid());
        }

        /// <inheritdoc />
        protected override void OnDetached()
        {
            if (Control is Android.Widget.TextView textView)
            {
                textView.SetShadowLayer(0, 0, 0, Android.Graphics.Color.Transparent);
            }
        }
    }
}
