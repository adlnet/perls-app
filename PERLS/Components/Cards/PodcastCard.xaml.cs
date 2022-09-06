using System;
using System.Collections.Generic;
using System.Windows.Input;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// A podcast card.
    /// </summary>
    public partial class PodcastCard : BaseCard
    {
        /// <summary>
        /// The select item command property.
        /// </summary>
        public static readonly BindableProperty SelectItemCommandProperty = BindableProperty.Create(nameof(SelectItemCommand), typeof(ICommand), typeof(CourseCard));

        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastCard"/> class.
        /// </summary>
        public PodcastCard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the select item command.
        /// </summary>
        /// <value>The select item command.</value>
        public ICommand SelectItemCommand
        {
            get => GetValue(SelectItemCommandProperty) as ICommand;
            set => SetValue(SelectItemCommandProperty, value);
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            var colorOne = Color.FromHex("#33000000");
            var colorTwo = (Color)App.Current.Resources["PodcastCardColor"];

            SKColor[] colors = { SKColors.Black, colorOne.ToSKColor(), colorTwo.ToSKColor(), colorTwo.ToSKColor(), colorTwo.ToSKColor() };
            SKShaderTileMode tileMode = SKShaderTileMode.Clamp;

            using (SKPaint paint = new SKPaint())
            {
                paint.Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, info.Height), colors, tileMode);
                canvas.DrawRect(info.Rect, paint);
            }
        }
    }
}
