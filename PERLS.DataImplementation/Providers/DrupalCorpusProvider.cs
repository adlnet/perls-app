using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Float.Core.Net;
using Float.FileDownloader;
using Newtonsoft.Json.Linq;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.DataImplementation.Models;
using Xamarin.Forms;

namespace PERLS.DataImplementation.Providers
{
    /// <inheritdoc />
    public class DrupalCorpusProvider : ICorpusProvider, IRemoteFileProvider, IRemoteProvider
    {
        readonly DrupalAPI drupalAPI;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrupalCorpusProvider"/> class.
        /// </summary>
        public DrupalCorpusProvider()
        {
            drupalAPI = DependencyService.Get<DrupalAPI>();
        }

        /// <inheritdoc />
        public Task<bool> IsReachable() => drupalAPI.IsReachable();

        /// <inheritdoc />
        public async Task<IEnumerable<IItem>> GetRecommendations()
        {
            var result = await drupalAPI.GetNodeList("api/recommendations").ConfigureAwait(false);

#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.OfflineAccess)
            {
                _ = DependencyService.Get<IOfflineContentService>().UpdateRecommendedItems(result);
            }
#pragma warning restore CS0162 // Unreachable code detected

            return result;
        }

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetTrendingContent() => drupalAPI.GetNodeList("api/trending");

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetRecentContent() => drupalAPI.GetNodeList("api/recent");

        /// <inheritdoc />
        public Task<IEnumerable<ITopic>> GetTopics() => Task.FromResult(Enumerable.Empty<ITopic>());

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetPodcasts()
        {
            try
            {
                return drupalAPI.GetNodeList("api/podcasts");
            }
            catch (Float.Core.Net.HttpRequestException e) when (e.Code == 404)
            {
                return Task.FromResult(Enumerable.Empty<IItem>());
            }
        }

        /// <inheritdoc />
        public async Task<ITaxonomyTerm> GetTaxonomyTerm(int termId)
        {
            var nodes = await drupalAPI.GetNodeList($"api/taxonomy/term/{termId}").ConfigureAwait(false);
            IEnumerable<ITaxonomyTerm> topics = nodes.Select(node => node.Topic);

            // with no dedicated taxonomy endpoint (yet) we just get all the content
            // associated with the term, smush together the tags and topics, and
            // then look up the term we're looking for via term ID
            var result = nodes
                .SelectMany(node => node.Tags)
                .Concat(topics)
                .FirstOrDefault(term => term.Tid == termId);

            if (result == default)
            {
                throw new DrupalCorpusProviderException($"Term not found: {termId}");
            }

            var stateProvider = DependencyService.Get<ILearnerStateProvider>();
            stateProvider.GetState(result).SetFollowingStatusUnknown();

            return result;
        }

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetEpisodes(IPodcast podcast) => drupalAPI.GetNodeList($"api/podcasts/{podcast?.Id}/episodes");

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetTopicContent(ITopic topic) => Task.FromResult(Enumerable.Empty<IItem>());

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> SearchCorpus(string query) => drupalAPI.GetNodeList("api/search", new Dictionary<string, string> { { "text", query } });

        /// <inheritdoc/>
        public async Task<IRemoteResource> GetResourceForUri(Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            return await drupalAPI.GetRemoteResource(uri.AbsoluteUri).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<IEnumerable<IItem>> GetTermItems(ITaxonomyTerm term)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            var stateProvider = DependencyService.Get<ILearnerStateProvider>();
            stateProvider.GetState(term).SetFollowingStatusUnknown();

            return drupalAPI.GetNodeList($"api/taxonomy/term/{term.Tid}");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IItemGroup>> GetRelevantContent()
        {
            var stateProvider = DependencyService.Get<ILearnerStateProvider>();
            var data = await drupalAPI.GetData<List<NodeGroup>>("api/browse").ConfigureAwait(false);
            var itemsToUpdate = data.SelectMany((arg) => arg.Items);
            await stateProvider.GetStateList(itemsToUpdate).ConfigureAwait(false);
            return data;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IItemGroup>> GetFollowedContent()
        {
            var stateProvider = DependencyService.Get<ILearnerStateProvider>();
            var data = await drupalAPI.GetData<List<NodeGroup>>("api/channels").ConfigureAwait(false);
            var itemsToUpdate = data.SelectMany((arg) => arg.Items);
            await stateProvider.GetStateList(itemsToUpdate).ConfigureAwait(false);
            return data;
        }

        /// <inheritdoc/>
        public async Task<IAppearance> GetAppearance()
        {
            return await drupalAPI.GetData<Appearance>("api/configuration-settings").ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ISection>> GetEnhancedDashboard()
        {
            return await drupalAPI.GetData<List<Section>>("api/dashboard").ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IRemoteResource>> GetResources(Uri endpoint, EntityType entityType)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            var relativeEndpoint = endpoint.IsAbsoluteUri ? endpoint.LocalPath : endpoint.OriginalString;

            switch (entityType)
            {
                case EntityType.Node:
                    return await drupalAPI.GetNodeList(relativeEndpoint).ConfigureAwait(false);
                case EntityType.TaxonomyTerm:
                    IEnumerable<IRemoteResource> remoteResources;

                    // special case: we expect a different format for the "following" endpoint
                    if (relativeEndpoint.EndsWith("following", StringComparison.OrdinalIgnoreCase))
                    {
                        remoteResources = await drupalAPI.GetData<IEnumerable<Tag>>(relativeEndpoint).ConfigureAwait(false);
                    }
                    else
                    {
                        remoteResources = await drupalAPI.GetData<PagedResponse<TaxonomyTerm>>(relativeEndpoint).ConfigureAwait(false);
                    }

                    return remoteResources;
                case EntityType.Group:
                    return await drupalAPI.GetData<PagedResponse<Group>>(relativeEndpoint).ConfigureAwait(false);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <inheritdoc />
        public Task<HttpRequestMessage> BuildRequestToDownloadFile(IRemoteFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var networkService = DependencyService.Get<INetworkConnectionService>();
            var authStrategy = networkService.AuthStrategy;

            if (authStrategy is OAuth2StrategyBase strategyBase)
            {
                strategyBase.GetAccessToken();
            }

            using (var message = new HttpRequestMessage(HttpMethod.Get, file.Url))
            {
                if (file is IFile existingFile && !string.IsNullOrEmpty(existingFile.ETag))
                {
                    message.Headers.IfNoneMatch.Add(new System.Net.Http.Headers.EntityTagHeaderValue(existingFile.ETag));
                }

                return authStrategy.AuthenticateRequest(message);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<FeatureFlag>> GetFeatureFlags()
        {
            var configuration = await drupalAPI.GetData<Dictionary<string, object>>("api/switches").ConfigureAwait(false);

            var features = configuration["switches"] as JObject;
            var properties = features.Properties();
            var featuresList = properties.Select((arg) =>
            {
                var enabled = (bool)((JValue)arg.Value["status"]).Value;

                return new FeatureFlag
                {
                    Name = new FeatureFlagName(arg.Name),
                    IsEnabled = enabled,
                };
            });

            return featuresList;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IItem>> GetNodeList(Uri endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            return await drupalAPI.GetData<PagedResponse<Node>>(endpoint.AbsolutePath).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<IGroup> GetGroup(int groupId) => drupalAPI.GetData<IGroup>($"group/{groupId}");

        /// <inheritdoc />
        public async Task<IEnumerable<IGroup>> GetGroups()
        {
            return await drupalAPI.GetData<PagedResponse<Group>>("api/groups").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IGroup>> GetJoinableGroups()
        {
            return await drupalAPI.GetData<PagedResponse<Group>>("api/groups?joinable=1").ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IItemGroup>> GetGroupTopics(IGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            return await drupalAPI.GetData<List<NodeGroup>>($"api/groups/{group.Gid}/topics").ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IItem>> GetGroupTopicContent(IGroup group, ITopic topic)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            return await drupalAPI.GetData<PagedResponse<Node>>($"api/groups/{group.Gid}/topics/{topic.Tid}").ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IItem>> GetGroupContent(IGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            return await drupalAPI.GetData<PagedResponse<Node>>($"api/groups/{group.Gid}/contents").ConfigureAwait(false);
        }
    }
}
