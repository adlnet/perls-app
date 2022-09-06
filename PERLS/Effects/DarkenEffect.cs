using System;
using Xamarin.Forms;

namespace PERLS.Effects
{
    /// <summary>
    /// The Darken Effect.
    /// </summary>
    public class DarkenEffect : RoutingEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DarkenEffect"/> class.
        /// </summary>
        public DarkenEffect() : base("PERLS.DarkenEffect")
        {
        }

        /// <summary>
        /// Gets or sets the DarkenMultiplier.
        /// </summary>
        /// <value>
        /// The DarkenMultiplier.
        /// </value>
        public double DarkenMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the corner radius for rounding the edges of the darkened frame.
        /// </summary>
        /// <value>
        /// The corner radius for rounding the edges of the darkened frame.
        /// </value>
        public double CornerRadius { get; set; }
    }
}
