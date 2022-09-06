using System;
using System.Globalization;
using Xamarin.Forms;

namespace PERLS.Data.Converters
{
    /// <summary>
    /// Converter which returns false if a string is not present.
    /// </summary>
    public class StringPresenceConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return !string.IsNullOrEmpty(str);
            }

            return false;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
