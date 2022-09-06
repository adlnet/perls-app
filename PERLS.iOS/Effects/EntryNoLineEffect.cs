using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(PERLS.iOS.Effects.EntryNoLineEffect), nameof(PERLS.Effects.EntryNoLineEffect))]

namespace PERLS.iOS.Effects
{
    /// <summary>
    /// Hides the border on text fields (Entry).
    /// </summary>
    public class EntryNoLineEffect : PlatformEffect
    {
        /// <inheritdoc />
        protected override void OnAttached()
        {
            if (Control is UITextField entry)
            {
                entry.BorderStyle = UITextBorderStyle.None;
            }
        }

        /// <inheritdoc />
        protected override void OnDetached()
        {
        }
    }
}
