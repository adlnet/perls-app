using FFImageLoading.Forms;
using Xamarin.Forms;

namespace PERLS.Data.ParagraphLayout.Components
{
    /// <summary>
    /// CachedImage with the ability to set maximum width.  Not efficient to set max width. Don't use unless neeeded.
    /// </summary>
    public class MaximumWidthImage : CachedImage
    {
        /// <summary>
        /// Identifies the <see cref="MaximumWidth"/> property.
        /// </summary>
        public static readonly BindableProperty MaximumWidthProperty = BindableProperty.Create(nameof(MaximumWidth), typeof(double), typeof(MaximumWidthImage), propertyChanged: MaximumWidthPropertyChanged);

        /// <summary>
        /// Initializes a new instance of the <see cref="MaximumWidthImage"/> class.
        /// </summary>
        public MaximumWidthImage()
        {
        }

        /// <summary>
        /// Gets or sets the maximum width of the image.
        /// </summary>
        /// <value>The maximum image width.</value>
        public double MaximumWidth
        {
            get => (double)GetValue(MaximumWidthProperty);
            set => SetValue(MaximumWidthProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnSizeAllocated(double width, double height)
        {
            if (width > MaximumWidth)
            {
                base.OnSizeAllocated(MaximumWidth, height);
            }
            else
            {
                base.OnSizeAllocated(width, height);
            }
        }

        private static void MaximumWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not CachedImage image)
            {
                return;
            }

            if (oldValue != newValue)
            {
               if (image.Width > (double)newValue)
                {
                    image.WidthRequest = (double)newValue;
                }
            }
        }
    }
}
