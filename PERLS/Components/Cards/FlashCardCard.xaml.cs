using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// Flash card card.
    /// </summary>
    public partial class FlashCardCard : FlippableCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlashCardCard"/> class.
        /// </summary>
        public FlashCardCard()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsFlipped))
            {
                FlipCardAsync();
            }
        }

        void MainScrollScrolled(object sender, ScrolledEventArgs e)
        {
            Recommendation.FadeOnScroll(e.ScrollY);
        }

        Task FlipCardAsync()
        {
            FrontOfCard.InputTransparent = IsFlipped;
            BackOfCard.InputTransparent = !IsFlipped;
            return FlipCard();
        }

        async Task FlipCard()
        {
            if (Parent is not VisualElement parent)
            {
                return;
            }

            if (parent.Parent is not VisualElement container)
            {
                return;
            }

            ViewExtensions.CancelAnimations(container);

            // 90.1 here wasn't done in error.  There seems to be a rendering issue in ios where the content doesn't appear if you change visibility at 90 degrees.
            await Task.WhenAny(
                    container.RotateYTo(90.1, 400, Easing.SinOut)).ConfigureAwait(true);

            BackOfCard.Opacity = IsFlipped ? 1 : 0;
            FrontOfCard.Opacity = IsFlipped ? 0 : 1;

            await Task.WhenAny(
                   container.RotateYTo(IsFlipped ? 180 : 0, 400, Easing.SinOut)).ConfigureAwait(true);
        }
    }
}
