using System;
using System.Globalization;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Converters
{
    /// <summary>
    /// View model to background color converter.
    /// </summary>
    public class ViewModelToBackgroundColorConverter : IValueConverter
    {
        /// <summary>
        /// Gets a default color to use when the model type is not supported.
        /// </summary>
        /// <value>The default color.</value>
        public Color DefaultColor => Color.Transparent;

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TeaserViewModel teaser)
            {
                return DefaultColor;
            }

            return teaser.ModelItem switch
            {
                IQuiz _ => Application.Current.Color("QuizColor"),
                ITip _ => Application.Current.Color("TipColor"),
                IFlashcard _ => Application.Current.Color("FlashCardColor"),
                _ => DefaultColor,
            };
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
