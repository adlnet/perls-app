using System;
using System.Globalization;
using Xamarin.Forms;

namespace PERLS.Data.Converters
{
    /// <summary>
    /// Reduces a multibinding of multiple booleans into a single true value if any are true.
    /// </summary>
    public class AnyTrueConverter : IMultiValueConverter
    {
        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || targetType?.IsAssignableFrom(typeof(bool)) != true)
            {
                return BindableProperty.UnsetValue;
            }

            foreach (var value in values)
            {
                if (value is bool b && b)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
