using System;
using System.Globalization;
using Xamarin.Forms;

namespace PERLS.Data.Converters
{
    /// <summary>
    /// Returns the given ConverterParameter if the bound value is not default; otherwise, returns default.
    /// </summary>
    public class ExistenceParameterConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == default ? default : parameter;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
