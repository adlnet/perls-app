using System;
using System.Globalization;
using Xamarin.Forms;

namespace PERLS.Data.Converters
{
    /// <summary>
    /// A converter that returns true if the object is not null, and false otherwise.
    /// </summary>
    /// <remarks>Consider moving into Float.Core.</remarks>
    public class ExistenceConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != default;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
