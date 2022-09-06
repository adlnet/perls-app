using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// A tip card.
    /// </summary>
    public partial class TipCard : BaseCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TipCard"/> class.
        /// </summary>
        public TipCard()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        protected override void OnSizeAllocated(double width, double height)
        {
            TipParagraph.MaximumWidth = width - 20;
            base.OnSizeAllocated(width, height);
        }

        private void MainScroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            Recommendation.FadeOnScroll(e.ScrollY);
        }
    }
}
