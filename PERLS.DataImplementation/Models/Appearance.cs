using System;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A container for the theme.
    /// </summary>
    [Serializable]
    public class Appearance : IAppearance
    {
        /// <summary>
        /// Gets the theme.
        /// </summary>
        /// <value>The theme.</value>
        [JsonProperty("theme")]
        public Theme Theme { get; internal set; }

        /// <inheritdoc/>
        [JsonIgnore]
        ITheme IAppearance.Theme => Theme;

        /// <summary>
        /// Gets the Logo.
        /// </summary>
        /// <value>The logo.</value>
        /// <remarks>
        /// The CMS allows for the content manager to upload a custom logo specific to the app.
        /// If an `app_logo` is specified, this returns the app logo.
        /// Otherwise, returns the `logo`.
        /// </remarks>
        public string Logo => CustomAppLogo?.OriginalString ?? CustomLogo?.OriginalString;

        [JsonProperty("logo")]
        internal Uri CustomLogo { get; set; }

        [JsonProperty("app_logo")]
        internal Uri CustomAppLogo { get; set; }
    }
}
