using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// A quiz card.
    /// </summary>
    public partial class QuizCard : BaseCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuizCard"/> class.
        /// </summary>
        public QuizCard()
        {
            InitializeComponent();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            using (SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                StrokeWidth = 4,
            })
            {
                canvas.DrawCircle(info.Width / 2, info.Height / 2, 15, paint);
            }
        }

        void DrawBigCircle(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            using (SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                StrokeWidth = 6,
            })
            {
                var circleRadius = (info.Width - paint.StrokeWidth) / 2f;
                canvas.DrawCircle(info.Width / 2, info.Height / 2, circleRadius, paint);
            }
        }
    }
}
