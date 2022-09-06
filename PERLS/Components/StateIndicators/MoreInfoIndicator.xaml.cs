using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PERLS.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// Recommendation indicator.
    /// </summary>
    public partial class MoreInfoIndicator : ContentView
    {
        /// <summary>
        /// The alternate color property.
        /// </summary>
        public static readonly BindableProperty AlternateColorProperty = BindableProperty.Create(nameof(AlternateColor), typeof(bool), typeof(ItemView));

        /// <summary>
        /// Initializes a new instance of the <see cref="MoreInfoIndicator"/> class.
        /// </summary>
        public MoreInfoIndicator()
        {
            InitializeComponent();
            Tint.HexColor = Application.Current.Color("ElementTintColor").ToHex();
            if (AlternateColor)
            {
                Tint.HexColor = Color.White.ToHex();
                RecommendationIcon.ReplaceStringMap = new Dictionary<string, string>
                {
                    { "#fff", "#66666666" },
                };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MoreInfoIndicator"/>
        /// alternate color.
        /// </summary>
        /// <value><c>true</c> if alternate color; otherwise, <c>false</c>.</value>
        public bool AlternateColor
        {
            get => (bool)GetValue(AlternateColorProperty);
            set => SetValue(AlternateColorProperty, value);
        }

        /// <summary>
        /// Fades the view on for scrolling.
        /// </summary>
        /// <param name="scrollPos">Scroll Position.</param>
        public void FadeOnScroll(double scrollPos)
        {
            if (Math.Abs(scrollPos) < 10 || scrollPos < 0)
            {
                this.Opacity = 1;
            }
            else if (scrollPos < 40)
            {
                this.Opacity = .9 / (scrollPos / 8);
            }
            else
            {
               this.Opacity = 0;
            }
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(AlternateColor))
            {
                if (AlternateColor)
                {
                    Tint.HexColor = Color.White.ToHex();
                    RecommendationIcon.ReplaceStringMap = new Dictionary<string, string>
                    {
                        { "#fff", "#666" },
                    };
                }
                else
                {
                    Tint.HexColor = ((Color)Application.Current.Resources["SecondaryColor"]).ToHex();
                    RecommendationIcon.ReplaceStringMap = new Dictionary<string, string>
                    {
                        { "#666", "#fff" },
                    };
                }
            }
        }

        /// <summary>
        /// Gets a command to flip the card.
        /// </summary>
        void ShowRecommendationReason(object sender, EventArgs args)
        {
            PopupNavigation.Instance.PushAsync(new MoreInfoPopupPage
            {
                BindingContext = this.BindingContext,
            });
        }
    }
}
