using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// Provides the component to toggle a bookmark on and off.
    /// </summary>
    public partial class BookmarkToggle : ContentView
    {
        /// <summary>
        /// The tint color property.
        /// </summary>
        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(BookmarkToggle));

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkToggle"/> class.
        /// </summary>
        public BookmarkToggle()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the tint color.
        /// </summary>
        /// <value>The tint color.</value>
        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set => SetValue(TintColorProperty, value);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(BindingContext))
            {
                if (TintColor != default)
                {
                    Image.Transformations = new List<ITransformation>
                    {
                        new TintTransformation()
                        {
                            EnableSolidColor = true,
                            HexColor = TintColor.ToHex(),
                        },
                    };
                }
            }
        }

        void OnTapped(object sender, EventArgs e)
        {
            if (sender is not Image ribbon)
            {
                return;
            }

            AnimateRibbonAsync(ribbon);
        }

        Task AnimateRibbonAsync(Image ribbon)
        {
            return AnimateRibbon(ribbon);
        }

        async Task AnimateRibbon(Image ribbon)
        {
            ViewExtensions.CancelAnimations(ribbon);

            var startingY = ribbon.Y;
            var distance = 10;

            _ = ribbon.ScaleTo(0.95, 150, Easing.CubicOut);
            await ribbon.TranslateTo(0, startingY - distance, 150).ConfigureAwait(false);

            _ = ribbon.ScaleTo(1, 150, Easing.CubicIn);
            await ribbon.TranslateTo(0, startingY, 150).ConfigureAwait(false);
        }
    }
}
