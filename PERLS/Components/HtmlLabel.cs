using PERLS.Data.Extensions;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// A label that has an <see cref="Html"/> property for displaying HTML.
    /// </summary>
    /// <remarks>
    /// When iOS displays HTML in a label, it completely ignores the styling already set on the label.
    /// This custom label takes some key styling elements of the label and generates a basic stylesheet
    /// so the text is rendered as expected.
    /// </remarks>
    public class HtmlLabel : Label
    {
        /// <summary>
        /// Identifies the <see cref="Html"/> property.
        /// </summary>
        public static readonly BindableProperty HtmlProperty = BindableProperty.Create(nameof(Html), typeof(string), typeof(HtmlLabel), propertyChanged: HandleHtmlPropertyChanged);

        /// <summary>
        /// Gets or sets the HTML text to display in this label.
        /// </summary>
        /// <value>The HTML to display in the label.</value>
        public string Html
        {
            get => (string)GetValue(HtmlProperty);
            set => SetValue(HtmlProperty, value);
        }

        static void HandleHtmlPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Label label && newValue is string html)
            {
                string styling = string.Empty;
                if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                {
                    styling = $"<style>body{{color: {label.TextColor.ToHtmlHex()}; font: {label.FontSize}px {label.FontFamily}; }} p {{margin: 0px; padding: 0px;}} strong {{color: {((Color)Application.Current.Resources["PrimaryColor"]).ToHtmlHex()};}}</style>";
                }

                label.TextType = TextType.Html;
                label.Text = styling + html;
            }
        }
    }
}
