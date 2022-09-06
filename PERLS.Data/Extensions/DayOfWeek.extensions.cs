using System;

namespace PERLS.Data.Extensions
{
    /// <summary>
    /// Class to extend the DayOfWeek enum.
    /// </summary>
    public static class DayOfWeekExtensions
    {
        /// <summary>
        /// Accesses the correct server string for the day.
        /// </summary>
        /// <param name="dayOfWeek">The DayOfWeek parameter being used for the extension.</param>
        /// <returns>The string for the server.</returns>
#pragma warning disable CA1308 // Normalize strings to uppercase
        public static string ToServerString(this DayOfWeek dayOfWeek) => Enum.GetName(typeof(DayOfWeek), dayOfWeek).ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

        /// <summary>
        /// Gets a DayOfWeek given a server string.
        /// </summary>
        /// <param name="serverString">The server string.</param>
        /// <returns>A DayOfWeek.</returns>
        public static DayOfWeek DayOfWeekFromServerString(this string serverString)
        {
            return (DayOfWeek)Enum.Parse(typeof(DayOfWeek), serverString.UppercaseFirstCharacter());
        }
    }
}
