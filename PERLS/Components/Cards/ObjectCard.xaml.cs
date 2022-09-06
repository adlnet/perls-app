using System;
using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// A generic object card.
    /// </summary>
    public partial class ObjectCard : BaseCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectCard"/> class.
        /// </summary>
        public ObjectCard()
        {
            InitializeComponent();

            // Sets the PrimaryTextColor back to default.
            Resources["PrimaryTextColor"] = Application.Current.Resources["PrimaryTextColor"];
        }

        /// <summary>
        /// Raised when the user taps on the card.
        /// </summary>
        /// <remarks>
        /// In my ideal world, this card should not have anything to do with
        /// handling it's own selection. However, it seems that having a scrollview
        /// within a card makes it impossible for the parent collection to
        /// disambiguate between the user scrolling the card or tapping the card.
        /// Because of that, we have to add a gesture recognizer _inside_
        /// the scrollview that can respond to a tap.
        /// </remarks>
        public event EventHandler Tapped
        {
            add
            {
                TapRecognizer.Tapped += value;
            }

            remove
            {
                TapRecognizer.Tapped -= value;
            }
        }

        /// <inheritdoc/>
        protected override void OnSizeAllocated(double width, double height)
        {
            if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Orientation == Xamarin.Essentials.DisplayOrientation.Landscape)
            {
                MainScroll.Orientation = ScrollOrientation.Vertical;
            }
            else
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    MainScroll.ScrollToAsync(0, 0, false);
                }

                MainScroll.Orientation = ScrollOrientation.Neither;
            }

            base.OnSizeAllocated(width, height);
        }
    }
}
