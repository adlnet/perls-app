using System;
using System.Globalization;
using PERLS;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PERLS.Services
{
    /// <summary>
    /// The Feature Flag Markup Extension.
    /// </summary>
    [ContentProperty("Flag")]
    public class FeatureFlagMarkupExtension : IMarkupExtension
    {
        const string PodcastSupport = "podcast_support";
        const string StatsSupport = "stats";
        const string NotesSupport = "notes";

        /// <summary>
        /// Gets or sets the Text.
        /// </summary>
        /// <value>The feature flag type.</value>
        public string Flag { get; set; }

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public IValueConverter Converter { get; set; }

        /// <inheritdoc/>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var featureFlagName = new FeatureFlagName(Flag);
            var service = DependencyService.Get<IFeatureFlagService>();
            var flagEnabled = Flag switch
            {
                PodcastSupport => Constants.Podcasts && service.IsFlagEnabled(featureFlagName),
                StatsSupport => Constants.StatsAccess,
                NotesSupport => Constants.NotesAccess,
                _ => service.IsFlagEnabled(featureFlagName),
            };

            if (Converter is IValueConverter converter)
            {
                return converter.Convert(flagEnabled, typeof(bool), null, CultureInfo.InvariantCulture);
            }
            else
            {
                return flagEnabled;
            }
        }
    }
}
