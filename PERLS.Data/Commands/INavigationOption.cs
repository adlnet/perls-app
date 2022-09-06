using Xamarin.Forms;

namespace PERLS.Data.Commands
{
    /// <summary>
    /// Defines a navigation option the user can select from.
    /// </summary>
    public interface INavigationOption
    {
        /// <summary>
        /// Gets or sets the label for the navigation option.
        /// </summary>
        /// <value>The label text.</value>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this show arrow.
        /// </summary>
        /// <value><c>true</c> if show arrow; otherwise, <c>false</c>.</value>
        bool ShowArrow { get; set; }

        /// <summary>
        /// Gets or sets the text color for this option.
        /// </summary>
        /// <value>The text color.</value>
        Color TextColor { get; set; }
    }

    /// <summary>
    /// Defines a navigation option the user can select from, with a typed destination.
    /// </summary>
    /// <typeparam name="T">The type of tha destination.</typeparam>
    public interface INavigationOption<T> : INavigationOption
    {
        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        T Destination { get; set; }
    }
}
