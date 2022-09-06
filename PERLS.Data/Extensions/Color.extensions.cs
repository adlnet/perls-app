using Xamarin.Forms;

namespace PERLS.Data.Extensions
{
    /// <summary>
    /// Allows convenient customizations of colors.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns the hex value for the color suitable for HTML.
        /// </summary>
        /// <remarks>
        /// In Xamarin, the alpha component is the first two characters;
        /// but in HTML, the alpha component is the last two characters.
        /// </remarks>
        /// <param name="color">The original color.</param>
        /// <returns>The hex value for the color with alpha component at the end (if it exists).</returns>
        public static string ToHtmlHex(this Color color)
        {
            var hexValue = color.ToHex();

            if (hexValue.Length == 9)
            {
                var alphaComponent = hexValue.Substring(1, 2);
                return $"#{hexValue.Substring(3)}{alphaComponent}";
            }

            return hexValue;
        }
    }
}
