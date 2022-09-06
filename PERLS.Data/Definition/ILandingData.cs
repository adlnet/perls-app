using System.ComponentModel;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The Landing data interface.
    /// </summary>
    public interface ILandingData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        /// Gets the landing image.
        /// </summary>
        /// <value>The description.</value>
        string LandingImage { get; }
    }
}
