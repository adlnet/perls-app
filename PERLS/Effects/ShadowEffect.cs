using Xamarin.Forms;

namespace PERLS.Effects
{
    /// <summary>
    /// Allows a shadow to be added to labels.
    /// On iOS, this will technically work with anything backed by a CALayer (basically any view).
    /// But on Android, the shadow layer is specific to a text view.
    /// </summary>
    /// <remarks>Inspired by the effect demo provided by Xamarin.</remarks>
    /// <see href="https://docs.microsoft.com/en-us/samples/xamarin/xamarin-forms-samples/effects-shadoweffect/"/>
    public class ShadowEffect : RoutingEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShadowEffect"/> class.
        /// </summary>
        public ShadowEffect() : base("PERLS.LabelShadowEffect")
        {
        }

        /// <summary>
        /// Gets or sets the density-independent radius of the shadow.
        /// </summary>
        /// <value>The shadow radius.</value>
        public float Radius { get; set; }

        /// <summary>
        /// Gets or sets the color of the shadow.
        /// </summary>
        /// <value>The shadow color.</value>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the x-offset of the shadow.
        /// </summary>
        /// <value>The shadow x-offset.</value>
        public float DistanceX { get; set; }

        /// <summary>
        /// Gets or sets the y-offset of the shadow.
        /// </summary>
        /// <value>The shadow y-offset.</value>
        public float DistanceY { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this effect is visible.
        /// </summary>
        /// <value><c>true</c> if the shadow is visible, <c>false</c> otherwise.</value>
        public bool IsVisible { get; set; }
    }
}
