using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Providers
{
    /// <summary>
    /// A cached variant of the <see cref="LearnerProvider"/> class.
    /// </summary>
    public class CachedLearnerProvider : BaseCachedProvider<LearnerProvider>, ILearnerProvider
    {
        const string FileName = "cached_learner_provider";

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedLearnerProvider"/> class.
        /// </summary>
        public CachedLearnerProvider() : base(new LearnerProvider(), new DiskProviderCache(FileName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedLearnerProvider"/> class.
        /// </summary>
        /// <param name="source">The source learner provider; this should not be a cached provider.</param>
        public CachedLearnerProvider(LearnerProvider source) : base(source, new DiskProviderCache(FileName))
        {
        }

        /// <inheritdoc />
        public Task<ILearner> GetCurrentLearner() => GetCachedResult(Source.GetCurrentLearner);

        /// <inheritdoc />
        public Task<ILearnerStats> GetCurrentLearnerStats() => GetCachedResult(Source.GetCurrentLearnerStats);

        /// <inheritdoc/>
        public Task<IEnumerable<IPrompt>> GetPrompts() => Source.GetPrompts(); // not cached

        /// <inheritdoc/>
        public Task SaveCurrentLearnerGoals() => Source.SaveCurrentLearnerGoals(); // not cached

        /// <inheritdoc/>
        public Task<IEnumerable<ICertificate>> GetCertificates() => GetCachedResult(Source.GetCertificates);

        /// <inheritdoc/>
        public async Task<ICertificate> GetCertificateItemFromId(string itemId)
        {
            var certificates = await GetCertificates().ConfigureAwait(false);

            if (certificates == null)
            {
                return null;
            }

            return certificates.FirstOrDefault((arg) => arg.UUID == itemId);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IBadge>> GetBadges() => GetCachedResult(Source.GetBadges);

        /// <inheritdoc/>
        public async Task<IBadge> GetBadgeItemFromId(string itemId)
        {
            var badges = await GetBadges().ConfigureAwait(false);

            if (badges == null)
            {
                return null;
            }

            return badges.FirstOrDefault((arg) => arg.UUID == itemId);
        }
    }
}
