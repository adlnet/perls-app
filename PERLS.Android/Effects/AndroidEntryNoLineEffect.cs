using PERLS.Droid.Effects;
using PERLS.Effects;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportEffect(typeof(AndroidEntryNoLineEffect), nameof(EntryNoLineEffect))]

namespace PERLS.Droid.Effects
{
    /// <summary>
    /// The Android implementation of the EntryNoLine effect.
    /// </summary>
    public class AndroidEntryNoLineEffect : PlatformEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidEntryNoLineEffect"/> class.
        /// </summary>
        public AndroidEntryNoLineEffect()
        {
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            if (Control == null)
            {
                return;
            }

            Control.SetBackground(null);
        }

        /// <inheritdoc/>
        protected override void OnDetached()
        {
        }
    }
}
