using System;
using FFImageLoading.Svg.Forms;
using Float.Core.ViewModels;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The Landing view model.
    /// </summary>
    public class LandingViewModel : ViewModel<ILandingData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LandingViewModel"/> class.
        /// </summary>
        /// <param name="model">The Model.</param>
        public LandingViewModel(ILandingData model) : base(model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            LandingImage = SvgImageSource.FromResource(model.LandingImage);
        }

        /// <summary>
        /// Gets the title label.
        /// </summary>
        /// <value>The title label.</value>
        public string TitleLabel => Model.Title;

        /// <summary>
        /// Gets the description label.
        /// </summary>
        /// <value>The description label.</value>
        public string DescriptionLabel => Model.Description;

        /// <summary>
        /// Gets the description label.
        /// </summary>
        /// <value>The description label.</value>
        public SvgImageSource LandingImage { get; }
    }
}
