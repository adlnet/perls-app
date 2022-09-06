using System;
using Xamarin.Forms;

namespace PERLS.Extensions
{
    /// <summary>
    /// Allows convenient customizations of colors.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns a color slighty darker than the receiver.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <param name="amount">The amount to darken it by (default=0.1).</param>
        /// <returns>A darker color.</returns>
        public static Color DarkerColor(this Color color, float amount = 0.1f)
        {
            return color.AddLuminosity(-Math.Abs(amount));
        }
    }
}
