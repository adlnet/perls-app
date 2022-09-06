using System;
using System.Globalization;
using Xamarin.Forms;

namespace PERLS.Data.Converters
{
    /// <summary>
    /// Converts a binding by decreasing it.
    /// </summary>
    public class DecreaseNumberConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - GetParameter(parameter);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + GetParameter(parameter);
        }

        double GetParameter(object parameter)
        {
            if (double.TryParse((string)parameter, out var result))
            {
                return result;
            }

            return 0;
        }
    }
}
