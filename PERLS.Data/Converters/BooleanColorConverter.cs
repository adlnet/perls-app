using System;
using System.Globalization;
using Xamarin.Forms;

namespace PERLS.Data.Converters
{
    /// <summary>
    /// Value converter that sets a color if the boolean is true.
    /// </summary>
    public class BooleanColorConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue && parameter is Color targetColor)
            {
                return targetColor;
            }

            return Color.Transparent;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
