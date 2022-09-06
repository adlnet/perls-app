using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PERLS.Data.Definition.Services;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Provides corpus content.
    /// </summary>
    public interface ICorpusProvider
    {
        /// <summary>
        /// Gets a list of recommendations for the current user.
        /// </summary>
        /// <returns>A list of recommendations.</returns>
        Task<IEnumerable<IItem>> GetRecommendations();

        /// <summary>
        /// Gets the currently trending content.
        /// </summary>
        /// <returns>The trending content.</returns>
        Task<IEnumerable<IItem>> GetTrendingContent();

        /// <summary>
        /// Gets content that was recently created.
        /// </summary>
        /// <returns>The recent content.</returns>
        Task<IEnumerable<IItem>> GetRecentContent();

        /// <summary>
        /// Gets a list of topics relevant to the user.
        /// </summary>
        /// <returns>A list of topics.</returns>
        Task<IEnumerable<ITopic>> GetTopics();

        /// <summary>
        /// Gets a list of podcasts for the current user.
        /// </summary>
        /// <returns>A list of podcasts.</returns>
        Task<IEnumerable<IItem>> GetPodcasts();

        /// <summary>
        /// Gets a taxonomy term, given the term's ID.
        /// </summary>
        /// <param name="termId">The term ID to disambiguate.</param>
        /// <returns>The full taxonomy term, or null if not found.</returns>
        Task<ITaxonomyTerm> GetTaxonomyTerm(int termId);

        /// <summary>
        /// Gets a list of podcast episodes for the current user.
        /// </summary>
        /// <returns>A list of episodes.</returns>
        /// <param name="podcast">The podcast for which episodes should be returned.</param>
        Task<IEnumerable<IItem>> GetEpisodes(IPodcast podcast);

        /// <summary>
        /// Gets topic content.
        /// </summary>
        /// <returns>The list of items in the topic.</returns>
        /// <param name="topic">The topic for which to get a list of items.</param>
        Task<IEnumerable<IItem>> GetTopicContent(ITopic topic);

        /// <summary>
        /// Gets the theme.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IAppearance> GetAppearance();

        /// <summary>
        /// Gets the enhanced dashboard, which is not supported by all tenants.
        /// </summary>
        /// <returns>An enumerable of sections to display in the dashboard.</returns>
        Task<IEnumerable<ISection>> GetEnhancedDashboard();

        /// <summary>
        /// Gets resources from a given server endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint, as a relative URI.</param>
        /// <param name="entityType">The type of entity to retrieve.</param>
        /// <returns>An enumerable of remote resources.</returns>
        Task<IEnumerable<IRemoteResource>> GetResources(Uri endpoint, EntityType entityType);

        /// <summary>
        /// Gets content that may be interesting/relevant to the current user.
        /// </summary>
        /// <returns>A list of groups containing interesting content.</returns>
        /// <remarks>
        /// This is different from recommendations in that it is not a personalized list.
        /// While the content returned here is relevant to the user, it is not necessarily
        /// unique to the user.
        /// </remarks>
        Task<IEnumerable<IItemGroup>> GetRelevantContent();

        /// <summary>
        /// Gets content that is followed by the current user.
        /// </summary>
        /// <returns>A list of groups containing followed content.</returns>
        Task<IEnumerable<IItemGroup>> GetFollowedContent();

        /// <summary>
        /// Searches the corpus for items that match the query.
        /// </summary>
        /// <returns>A list of search results.</returns>
        /// <param name="query">The query for which to search.</param>
        Task<IEnumerable<IItem>> SearchCorpus(string query);

        /// <summary>
        /// Gets the resource given a URI.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <returns>The resource matching the Uri.</returns>
        Task<IRemoteResource> GetResourceForUri(Uri uri);

        /// <summary>
        /// Gets the items associated with the specified term ID.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The items associated with the term.</returns>
        Task<IEnumerable<IItem>> GetTermItems(ITaxonomyTerm term);

        /// <summary>
        /// Gets the feature flags.
        /// </summary>
        /// <returns>The feature flags.</returns>
        Task<IEnumerable<FeatureFlag>> GetFeatureFlags();

        /// <summary>
        /// Gets the node list.
        /// </summary>
        /// <param name="endpoint">The endpoint, as a relative Uri.</param>
        /// <returns>The node list.</returns>
        Task<IEnumerable<IItem>> GetNodeList(Uri endpoint);

        /// <summary>
        /// Gets a specified group.
        /// </summary>
        /// <param name="groupId">The id of the group to retrieve.</param>
        /// <returns>The specified group.</returns>
        Task<IGroup> GetGroup(int groupId);

        /// <summary>
        /// Gets a list of group states for the current user.
        /// </summary>
        /// <returns>A list of groups.</returns>
        Task<IEnumerable<IGroup>> GetGroups();

        /// <summary>
        /// Gets a list of joinable group states for the current user.
        /// </summary>
        /// <returns>A list of joinable groups.</returns>
        Task<IEnumerable<IGroup>> GetJoinableGroups();

        /// <summary>
        /// Gets a list of topics related to the given group.
        /// </summary>
        /// <param name="group">The group for which to get topics.</param>
        /// <returns>A list of topics for the group.</returns>
        Task<IEnumerable<IItemGroup>> GetGroupTopics(IGroup group);

        /// <summary>
        /// Gets a list of content related to a topic within a group.
        /// </summary>
        /// <param name="group">The group for which to find content.</param>
        /// <param name="topic">The topic for which to find content.</param>
        /// <returns>The items in the group's topic.</returns>
        Task<IEnumerable<IItem>> GetGroupTopicContent(IGroup group, ITopic topic);

        /// <summary>
        /// Gets a list of content within a group.
        /// </summary>
        /// <param name="group">The group for which to find content.</param>
        /// <returns>The items in the group.</returns>
        Task<IEnumerable<IItem>> GetGroupContent(IGroup group);
    }
}
