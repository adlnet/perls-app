using System;
using System.Globalization;

namespace PERLS.Data.Extensions
{
    /// <summary>
    /// Allows formatting of strings.
    /// </summary>
    public static class StringsExtensions
    {
        /// <summary>
        /// Formats the string to contain the current app name.
        /// </summary>
        /// <param name="stringRef">The string to format.</param>
        /// <returns>A formatted string with the app name.</returns>
        public static string AddAppName(this string stringRef)
        {
            if (stringRef != null && stringRef.Contains("{0}"))
            {
                return string.Format(CultureInfo.InvariantCulture, stringRef, Xamarin.Essentials.AppInfo.Name);
            }

            return stringRef;
        }

        /// <summary>
        /// Formats the string to uppercase the first character, ignoring culture.
        /// </summary>
        /// <param name="stringRef">The string to format.</param>
        /// <returns>The formatted string.</returns>
        public static string UppercaseFirstCharacter(this string stringRef)
        {
            if (string.IsNullOrWhiteSpace(stringRef))
            {
                return stringRef;
            }

            return $"{char.ToUpperInvariant(stringRef[0])}{stringRef.Substring(1)}";
        }
    }
}
