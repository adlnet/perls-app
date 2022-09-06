using PERLS.Data.Infrastructure;
using PERLS.Data.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// A visual representation of a single content item with an image and a title.
    /// </summary>
    public partial class Tile : ItemView
    {
        /// <summary>
        /// Identifies the <see cref="ShouldShowGradient"/> property.
        /// </summary>
        public static readonly BindableProperty ShouldShowGradientProperty = BindableProperty.Create(nameof(ShouldShowGradientProperty), typeof(bool), typeof(Tile));

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class.
        /// </summary>
        public Tile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the canvas should be painted.
        /// </summary>
        /// <value>A bool value.</value>
        public bool ShouldShowGradient
        {
            get => (bool)GetValue(ShouldShowGradientProperty);
            set => SetValue(ShouldShowGradientProperty, value);
        }

        /// <inheritdoc />
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            CanvasView.InvalidateSurface();
        }

        /// <summary>
        /// Handles painting the SKCanvasView.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">SKPaintSurface event args.</param>
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var rect = args.Info.Rect;
            var surface = args.Surface;
            surface.Canvas.Clear();

            var colorOne = Color.FromHex("#A8000000");
            var colorTwo = Color.FromHex("#00FFFFFF");

            if (BindingContext is TeaserViewModel teaser)
            {
                colorTwo = teaser.GradientColor;
            }

            SKColor[] colors = { colorOne.ToSKColor(), Color.Transparent.ToSKColor() };
            SKColor[] colors2 = { Color.Transparent.ToSKColor(), colorTwo.MultiplyAlpha(.95).ToSKColor() };
            var tileMode = SKShaderTileMode.Clamp;

            using (SKPaint paint = new SKPaint())
            {
                paint.Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, rect.Height), colors, tileMode);
                surface.Canvas.DrawRect(rect, paint);

                paint.Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, rect.Height), colors2, new float[] { 0, 0.9f }, tileMode);
                surface.Canvas.DrawRect(rect, paint);
            }
        }
    }
}
