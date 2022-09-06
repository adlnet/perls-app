using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The theme.
    /// </summary>
    [Serializable]
    public class Theme : ITheme
    {
        const string PrimaryColorKey = "primary";

        const string CourseCardColorKey = "course";

        const string QuizColorKey = "quiz";

        const string FlashCardColorKey = "flash";

        const string TipColorKey = "tip";

        const string SecondaryColorKey = "secondary";

        const string TertiaryColorKey = "tertiary";

        const string PodcastColorKey = "podcast";

        const string BackgroundColorKey = "base";

        const string PrimaryTextColorKey = "text";

        /// <inheritdoc/>
        [JsonProperty("palette")]
        public Dictionary<string, string> Palette { get; internal set; }

        /// <inheritdoc/>
        public string PrimaryColor => Palette[PrimaryColorKey];

        /// <inheritdoc/>
        public string CourseCardColor => Palette[CourseCardColorKey];

        /// <inheritdoc/>
        public string QuizColor => Palette[QuizColorKey];

        /// <inheritdoc/>
        public string TipColor => Palette[TipColorKey];

        /// <inheritdoc/>
        public string SecondaryColor => Palette[SecondaryColorKey];

        /// <inheritdoc/>
        public string FlashCardColor => Palette[FlashCardColorKey];

        /// <inheritdoc/>
        public string TertiaryColor => Palette[TertiaryColorKey];

        /// <inheritdoc/>
        public string PodcastCardColor => Palette[PodcastColorKey];

        /// <inheritdoc/>
        public string BackgroundColor => Palette[BackgroundColorKey];

        /// <inheritdoc/>
        public string PrimaryTextColor => Palette[PrimaryTextColorKey];

        /// <inheritdoc/>
        public Dictionary<string, string> ColorNameForKeys
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { PrimaryColorKey, nameof(PrimaryColor) },
                    { CourseCardColorKey, nameof(CourseCardColor) },
                    { QuizColorKey, nameof(QuizColor) },
                    { TipColorKey, nameof(TipColor) },
                    { SecondaryColorKey, nameof(SecondaryColor) },
                    { FlashCardColorKey, nameof(FlashCardColor) },
                    { TertiaryColorKey, nameof(TertiaryColor) },
                    { PodcastColorKey, nameof(PodcastCardColor) },
                    { PrimaryTextColorKey, nameof(PrimaryTextColor) },
                    { BackgroundColorKey, nameof(BackgroundColor) },
                };
            }
        }
    }
}
