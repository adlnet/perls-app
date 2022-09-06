using System;
using System.Globalization;
using Xamarin.Forms;

namespace PERLS.Data.Converters
{
    /// <summary>
    /// A converter that adds an octothorpe to strings without one.
    /// </summary>
    public class OctothorpeConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                if (!text.Contains("#"))
                {
                    return "#" + text;
                }
                else
                {
                    return text;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
