using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// A card that is flippable.
    /// </summary>
    public abstract class FlippableCard : BaseCard
    {
        /// <summary>
        /// Identifies the <see cref="IsFlipped"/> property.
        /// </summary>
        public static readonly BindableProperty IsFlippedProperty = BindableProperty.Create(nameof(IsFlipped), typeof(bool), typeof(FlippableCard));

        /// <summary>
        /// Initializes a new instance of the <see cref="FlippableCard"/> class.
        /// </summary>
        protected FlippableCard() : base()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the card is flipped.
        /// </summary>
        /// <value><c>true</c> if the card is flipped, <c>false</c> otherwise.</value>
        public bool IsFlipped
        {
            get => (bool)GetValue(IsFlippedProperty);
            set
            {
                if (value != IsFlipped)
                {
                    SetValue(IsFlippedProperty, value);
                }
            }
        }
    }
}
