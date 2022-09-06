using System.Threading.Tasks;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// An interface for the feature flag service.
    /// </summary>
    public interface IFeatureFlagService
    {
        /// <summary>
        /// A value indicating whether this feature flag is enabled.
        /// </summary>
        /// <param name="featureFlag">The feature flag.</param>
        /// <returns>true if the feature flag is enabled, false otherwise.</returns>
        bool IsFlagEnabled(FeatureFlagName featureFlag);

        /// <summary>
        /// Updates the feature flags.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>An awaitable task.</returns>
        Task UpdateFlagStatus(ICorpusProvider provider);
    }
}
