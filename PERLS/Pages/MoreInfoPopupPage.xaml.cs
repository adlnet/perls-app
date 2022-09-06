using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

namespace PERLS.Pages
{
    /// <summary>
    /// Recommendation popup page.
    /// </summary>
    public partial class MoreInfoPopupPage : PopupPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoreInfoPopupPage"/> class.
        /// </summary>
        public MoreInfoPopupPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);

            // Give the scrollview a proper height that will look good.
            var bufferSpace = 100.0;

            var floor = 200.0;
            var ceiling = height - bufferSpace;

            if (contentStackLayout.Height < floor)
            {
                scrollView.HeightRequest = floor;
            }
            else if (contentStackLayout.Height > ceiling)
            {
                scrollView.HeightRequest = ceiling;
            }
            else
            {
                scrollView.HeightRequest = contentStackLayout.Height;
            }
        }

        void CloseCommand(object sender, EventArgs args)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}
