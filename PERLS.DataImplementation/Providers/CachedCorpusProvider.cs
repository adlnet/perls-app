using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Extensions;

namespace PERLS.DataImplementation.Providers
{
    /// <summary>
    /// A cached variant of the <see cref="DrupalCorpusProvider"/> class.
    /// </summary>
    public class CachedCorpusProvider : BaseCachedProvider<DrupalCorpusProvider>, ICorpusProvider
    {
        const string FileName = "cached_corpus_provider";
        bool ticket4590workaround = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCorpusProvider"/> class.
        /// </summary>
        public CachedCorpusProvider() : base(new DrupalCorpusProvider(), new DiskProviderCache(FileName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCorpusProvider"/> class.
        /// </summary>
        /// <param name="source">The source learner state provider; this should not be a cached provider.</param>
        public CachedCorpusProvider(DrupalCorpusProvider source) : base(source, new DiskProviderCache(FileName))
        {
        }

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetRecentContent() => GetCachedResult(Source.GetRecentContent);

        /// <inheritdoc />
        public async Task<IEnumerable<IItem>> GetRecommendations()
        {
            return await GetCachedResult(Source.GetRecommendations).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<IEnumerable<IItemGroup>> GetRelevantContent() => GetCachedResult(Source.GetRelevantContent);

        /// <inheritdoc />
        public Task<IEnumerable<IItemGroup>> GetFollowedContent() => GetCachedResult(Source.GetFollowedContent);

        /// <inheritdoc />
        public async Task<IEnumerable<IItem>> GetTermItems(ITaxonomyTerm term)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            if (await Source.IsReachable().ConfigureAwait(false))
            {
                return await GetCachedResult(Source.GetTermItems(term), caller: $"GetTermItems+{term.Tid}").ConfigureAwait(false);
            }

            return (await GetCachedCatalog().ConfigureAwait(false))
                .Where(item => item.Topic?.Tid == term.Tid || item.Tags?.Any(tag => tag.Tid == term.Tid) == true)
                .AsCacheDerived();
        }

        /// <inheritdoc/>
        public Task<IAppearance> GetAppearance() => GetCachedResult(Source.GetAppearance());

        /// <inheritdoc/>
        public Task<IEnumerable<ISection>> GetEnhancedDashboard() => GetCachedResult(Source.GetEnhancedDashboard);

        /// <inheritdoc/>
        public async Task<IEnumerable<IRemoteResource>> GetResources(Uri endpoint, EntityType entityType)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            // this is a "temporary" workaround for #SL-4590
            if (ticket4590workaround)
            {
                await Task.Delay(250).ConfigureAwait(false);
                ticket4590workaround = false;
            }

            return await GetCachedResult(Source.GetResources(endpoint, entityType), caller: $"GetResources+{endpoint}+{entityType}").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetTopicContent(ITopic topic)
        {
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            return GetCachedResult(Source.GetTopicContent(topic), caller: $"GetTopicContent+{topic.Id}");
        }

        /// <inheritdoc />
        public Task<IEnumerable<ITopic>> GetTopics() => GetCachedResult(Source.GetTopics);

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetTrendingContent() => GetCachedResult(Source.GetTrendingContent);

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetPodcasts() => GetCachedResult(Source.GetPodcasts);

        /// <inheritdoc/>
        public Task<IGroup> GetGroup(int groupId) => GetCachedResult(Source.GetGroup(groupId));

        /// <inheritdoc />
        public Task<ITaxonomyTerm> GetTaxonomyTerm(int termId) => GetCachedResult(Source.GetTaxonomyTerm(termId), caller: $"GetTaxonomyTerm+{termId}");

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetEpisodes(IPodcast podcast)
        {
            if (podcast == null)
            {
                throw new ArgumentNullException(nameof(podcast));
            }

            return GetCachedResult(Source.GetEpisodes(podcast), caller: $"GetEpisodes+{podcast.Id}");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IItem>> SearchCorpus(string query)
        {
            if (await Source.IsReachable().ConfigureAwait(false))
            {
                return await Source.SearchCorpus(query).ConfigureAwait(false);
            }

            return (await GetCachedCatalog().ConfigureAwait(false))
                .Search(query)
                .AsCacheDerived();
        }

        /// <inheritdoc/>
        public async Task<IRemoteResource> GetResourceForUri(Uri uri)
        {
            if (uri?.AbsolutePath is not string path || string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Invalid URI");
            }

            var cachedValue = (await GetCachedCatalog().ConfigureAwait(false))
                .FirstOrDefault(arg => arg?.Url?.AbsolutePath == path);

            if (cachedValue != null)
            {
                return cachedValue;
            }

            return await Source.GetResourceForUri(uri).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<FeatureFlag>> GetFeatureFlags() => Source.GetFeatureFlags();

        /// <inheritdoc/>
        public Task<IEnumerable<IItem>> GetNodeList(Uri endpoint) => Source.GetNodeList(endpoint);

        /// <inheritdoc/>
        public Task<IEnumerable<IGroup>> GetGroups() => GetCachedResult(Source.GetGroups);

        /// <inheritdoc/>
        public Task<IEnumerable<IGroup>> GetJoinableGroups() => GetCachedResult(Source.GetJoinableGroups);

        /// <inheritdoc/>
        public Task<IEnumerable<IItemGroup>> GetGroupTopics(IGroup group) => GetCachedResult(Source.GetGroupTopics(group), caller: $"GetGroupTopics+{group?.Gid}");

        /// <inheritdoc/>
        public Task<IEnumerable<IItem>> GetGroupTopicContent(IGroup group, ITopic topic) => GetCachedResult(Source.GetGroupTopicContent(group, topic), caller: $"GetGroupTopicContent+{group?.Gid},{topic?.Tid}");

        /// <inheritdoc/>
        public Task<IEnumerable<IItem>> GetGroupContent(IGroup group) => GetCachedResult(Source.GetGroupContent(group), caller: $"GetGroupContent+{group?.Gid}");

        Task<IEnumerable<IItem>> GetCachedCatalog()
        {
            return Task.Run(() =>
            {
                return DiskProviderCache
                    .KnownCacheFiles()
                    .Select(filePath => new DiskProviderCache(new FileInfo(filePath).Name))
                    .SelectMany(store =>
                    {
                        var storeItems = store
                            .Keys?
                            .Select(key =>
                            {
                                // Iterate over each key in the store to see if it's containing a list of items,
                                if (store.Get<IEnumerable<IItem>>(key) is IEnumerable<IItem> items)
                                {
                                    return items;
                                }

                                // or a list of list of items.
                                if (store.Get<IEnumerable<IItemGroup>>(key) is IEnumerable<IItemGroup> itemGroup)
                                {
                                    return itemGroup.SelectMany(group => group.Items);
                                }

                                return Enumerable.Empty<IItem>();
                            })
                            .SelectMany(items =>
                            {
                                var courseItems = items
                                    .Where(item => item is ICourse)
                                    .SelectMany(course => (course as ICourse).LearningObjects);

                                return items
                                    .Union(courseItems);
                            });

                        return storeItems ?? Enumerable.Empty<IItem>();
                    })
                    .Distinct();
            });
        }
    }
}
