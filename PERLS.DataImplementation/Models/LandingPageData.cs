using System;
using System.ComponentModel;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// Landing page data.
    /// </summary>
    [Serializable]
    public class LandingPageData : ILandingData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LandingPageData"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="description">The Description.</param>
        /// <param name="onboardingImageName">The landing page image file name.</param>
        public LandingPageData(string title, string description, string onboardingImageName)
        {
            Title = title;
            Description = description;
            LandingImage = onboardingImageName;
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Gets the Title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; }

        /// <summary>
        /// Gets the Description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; }

        /// <summary>
        /// Gets the Landing page image.
        /// </summary>
        /// <value>The landing page image.</value>
        public string LandingImage { get; }
    }
}
