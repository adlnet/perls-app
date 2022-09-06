using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PERLS.Data
{
    /// <summary>
    /// Converts date/time.
    /// </summary>
    public class DateTimeOffsetConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeOffsetConverter"/> class.
        /// </summary>
        public DateTimeOffsetConverter()
        {
            DateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz";
            Culture = CultureInfo.GetCultureInfo("en-US", "en");
        }
    }
}
