using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.DataImplementation.Providers;

namespace PERLS.Services
{
    /// <summary>
    /// The feature flag service implementation.
    /// </summary>
    public class FeatureFlagService : IFeatureFlagService, ICachedProvider
    {
        const string FeatureFlagStorageName = "feature_flags";
        readonly DiskProviderCache flagStorage = new DiskProviderCache(FeatureFlagStorageName);

        IEnumerable<FeatureFlag> AllFeatureFlags
        {
            get
            {
                if (flagStorage.Values is IEnumerable<object> values)
                {
                    return values.OfType<FeatureFlag>();
                }

                return Enumerable.Empty<FeatureFlag>();
            }
        }

        /// <inheritdoc/>
        public void ClearCache()
        {
            flagStorage.Clear();
        }

        /// <inheritdoc/>
        public bool IsFlagEnabled(FeatureFlagName featureFlag)
        {
            var flag = AllFeatureFlags.FirstOrDefault((arg) => arg.Name.Equals(featureFlag));
            return flag?.IsEnabled ?? false;
        }

        /// <inheritdoc/>
        public async Task UpdateFlagStatus(ICorpusProvider provider)
        {
            var flags = await provider.GetFeatureFlags().ConfigureAwait(false);
            flags.ForEach(arg => flagStorage.Put(arg.Name.ToString(), arg));
        }
    }
}
