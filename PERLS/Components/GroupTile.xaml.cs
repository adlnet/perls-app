using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// A visual representation of a single content item with an image and a title.
    /// </summary>
    public partial class GroupTile : ItemView
    {
        /// <summary>
        /// Identifies the <see cref="IsJoinable"/> property.
        /// </summary>
        public static readonly BindableProperty IsJoinableProperty = BindableProperty.Create(nameof(IsJoinable), typeof(bool), typeof(GroupTile));

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupTile"/> class.
        /// </summary>
        public GroupTile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the the group tiles are joinable.
        /// </summary>
        /// <value>A bool value.</value>
        public bool IsJoinable
        {
            get => (bool)GetValue(IsJoinableProperty);
            set => SetValue(IsJoinableProperty, value);
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

            SKColor[] colors = { colorOne.ToSKColor(), Color.Transparent.ToSKColor() };
            var tileMode = SKShaderTileMode.Clamp;

            using (SKPaint paint = new SKPaint())
            {
                paint.Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, rect.Height), colors, tileMode);
                surface.Canvas.DrawRect(rect, paint);
            }
        }
    }
}
