using System;
using System.Globalization;

namespace PERLS.Data.Extensions
{
    /// <summary>
    /// Allows convenient formatting of time elements.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Convert a TimeSpan to a string for representing the duration of a media track.
        /// </summary>
        /// <returns>The formatted string.</returns>
        /// <param name="span">Time span.</param>
        public static string ToDurationString(this TimeSpan span)
        {
            if (span.TotalHours >= 1)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0}{1} {2}{3}", (int)Math.Round(span.TotalHours), Strings.HrLabel, span.Minutes, Strings.MinLabel);
            }
            else if (span.TotalMinutes >= 1)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0}{1}", (int)Math.Round(span.TotalMinutes), Strings.MinLabel);
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, "{0}{1}", span.Seconds, Strings.SecLabel);
            }
        }

        /// <summary>
        /// Convert a number of seconds to a string for representing the duration of a media track.
        /// </summary>
        /// <returns>The formatted string.</returns>
        /// <param name="seconds">The seconds for a time span.</param>
        public static string ToDurationStringFromSeconds(this int seconds)
        {
            return ToDurationString(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// Round a double to the hundredth's place.
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <returns>The rounded value.</returns>
        public static double ToRoundedHundredths(this double value)
        {
            return Math.Round(value, 2);
        }
    }
}
