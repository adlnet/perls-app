using System;
using Xamarin.Forms;

namespace PERLS.Components.Cards
{
    /// <summary>
    /// The Course Description Card.
    /// </summary>
    public partial class CourseDescriptionCard : BaseCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CourseDescriptionCard"/> class.
        /// </summary>
        public CourseDescriptionCard()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (Device.RuntimePlatform == Device.Android)
            {
                if (height > 0)
                {
                    DescriptionLabel.MaxLines = (int)Math.Floor((DescriptionLabel.Height - 8) / (DescriptionLabel.FontSize + 5));
                }
            }
        }
    }
}
