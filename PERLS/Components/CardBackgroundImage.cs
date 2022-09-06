using System;
using FFImageLoading.Transformations;
using PERLS.Converters;
using PERLS.Data.Converters;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// The card background image.
    /// </summary>
    public class CardBackgroundImage : FFImageLoading.Forms.CachedImage
    {
        /// <summary>
        /// The max lines property.
        /// </summary>
        public static readonly BindableProperty ImageColorProperty = BindableProperty.Create(nameof(ImageColor), typeof(Color), typeof(CardBackgroundImage), propertyChanged: ImageColorPropertyChanged);

        /// <summary>
        /// Initializes a new instance of the <see cref="CardBackgroundImage"/> class.
        /// </summary>
        public CardBackgroundImage()
        {
            this.Opacity = .7;
        }

        /// <summary>
        /// Gets or sets the max lines.
        /// </summary>
        /// <value>The max lines.</value>
        public Color ImageColor
        {
            get => (Color)GetValue(ImageColorProperty);
            set => SetValue(ImageColorProperty, value);
        }

        private static void ImageColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var converter = new AltColorConverter();
            var newColor = (Color)converter.Convert((Color)newValue, null, null, null);

            var transformToWhite = new TintTransformation(255, 255, 255, 255);
            var transformToImageColor = new TintTransformation((int)(newColor.R * 255), (int)(newColor.G * 255), (int)(newColor.B * 255), 255);
            transformToWhite.EnableSolidColor = true;
            transformToImageColor.EnableSolidColor = true;
            var image = (FFImageLoading.Forms.CachedImage)bindable;
            image.Transformations.Add(transformToWhite);
            image.Transformations.Add(transformToImageColor);
        }
    }
}
