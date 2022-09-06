using System.Collections.Generic;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The theme interface.
    /// </summary>
    public interface ITheme
    {
        /// <summary>
        /// Gets the palette.
        /// </summary>
        /// <value>The palette.</value>
        Dictionary<string, string> Palette { get; }

        /// <summary>
        /// Gets a Dictionary that contains the name of the color for each key.
        /// </summary>
        /// <value>The course color.</value>
        Dictionary<string, string> ColorNameForKeys { get; }

        /// <summary>
        /// Gets the primary color.
        /// </summary>
        /// <value>The primary color.</value>
        string PrimaryColor { get; }

        /// <summary>
        /// Gets a course color.
        /// </summary>
        /// <value>The course color.</value>
        string CourseCardColor { get; }

        /// <summary>
        /// Gets a quiz color.
        /// </summary>
        /// <value>The quiz color.</value>
        string QuizColor { get; }

        /// <summary>
        /// Gets a Tip color.
        /// </summary>
        /// <value>The course color.</value>
        string TipColor { get; }

        /// <summary>
        /// Gets a Secondary color.
        /// </summary>
        /// <value>The course color.</value>
        string SecondaryColor { get; }

        /// <summary>
        /// Gets a flash card color.
        /// </summary>
        /// <value>The flash card.</value>
        string FlashCardColor { get; }

        /// <summary>
        /// Gets a tertiary color.
        /// </summary>
        /// <value>The flash card.</value>
        string TertiaryColor { get; }

        /// <summary>
        /// Gets the podcast color.
        /// </summary>
        /// <value>The podcast color.</value>
        string PodcastCardColor { get; }

        /// <summary>
        /// Gets the background color.
        /// </summary>
        /// <value>
        /// The background color.
        /// </value>
        string BackgroundColor { get; }

        /// <summary>
        /// Gets the primary text color.
        /// </summary>
        /// <value>The primary text color.</value>
        string PrimaryTextColor { get; }
    }
}
