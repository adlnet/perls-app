using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Analytics;
using Float.Core.Collections;
using Float.Core.Extensions;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ExperienceAPI;
using PERLS.Data.Extensions;
using PERLS.DataImplementation.Models;
using Xamarin.Forms;

namespace PERLS.DataImplementation.Providers
{
    /// <inheritdoc />
    public class LearnerStateProvider : BaseExperienceAPILearnerStateProvider, IRemoteProvider
    {
        const string BookmarksCacheKey = "bookmarks";
        const string HistoryCacheKey = "history";
        readonly DrupalAPI drupalAPI;
        readonly ObservableElementCollection<IItem> bookmarks = new ObservableElementCollection<IItem>();
        readonly ObservableElementCollection<IItem> history = new ObservableElementCollection<IItem>();
        readonly RepeatableTaskContainer<IEnumerable<IItem>> bookmarkTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerStateProvider"/> class.
        /// </summary>
        public LearnerStateProvider() : base(new DiskProviderCache("learner-state"))
        {
            drupalAPI = DependencyService.Get<DrupalAPI>();

            bookmarkTask = new RepeatableTaskContainer<IEnumerable<IItem>>(async () =>
            {
                if (await PersistAndGetLearnerState(drupalAPI.GetNodeList("api/bookmarks"), bookmarks.Any()).ConfigureAwait(false) is IEnumerable<IItem> updatedBookmarks)
                {
                    StateManager.UpdateBookmarkedItems(updatedBookmarks, bookmarks);

                    var bookmarksToRemove = bookmarks.Except(updatedBookmarks).ToList();
                    var bookmarksToAdd = updatedBookmarks.Except(bookmarks).ToList();
                    bookmarks.RemoveRange(bookmarksToRemove);
                    bookmarks.AddRange(bookmarksToAdd);

                    HandleBookmarksChanged();
                }

                return bookmarks;
            });

            if (PersistentStore.ContainsKey(BookmarksCacheKey))
            {
                var cachedBookmarks = PersistentStore.Get<IEnumerable<IItem>>(BookmarksCacheKey);
                StateManager.UpdateBookmarkedItems(cachedBookmarks, bookmarks);
                bookmarks.AddRange(cachedBookmarks);
            }

            if (PersistentStore.ContainsKey(HistoryCacheKey))
            {
                var cachedHistory = PersistentStore.Get<IEnumerable<IItem>>(HistoryCacheKey);
                history.AddRange(cachedHistory);
            }
        }

        /// <inheritdoc />
        public Task<bool> IsReachable() => drupalAPI.IsReachable();

        /// <inheritdoc />
        public override Task<IEnumerable<IItem>> GetBookmarks() => bookmarkTask.Run();

        /// <inheritdoc />
        public override async Task<IEnumerable<IItem>> GetHistory()
        {
            if (await PersistAndGetLearnerState(drupalAPI.GetNodeList("api/history"), history.Any()).ConfigureAwait(false) is List<Node> updatedHistory)
            {
                var historyToRemove = history.Except(updatedHistory).ToList();
                var historyToAdd = updatedHistory.Except(history).ToList();
                history.RemoveRange(historyToRemove);
                history.AddRange(historyToAdd);

                HandleHistoryChanged();
            }

            return history;
        }

        /// <inheritdoc/>
        public override Task DeleteAnnotation(IAnnotation annotation)
        {
            ReportingService.ReportAnnotationDeleted(annotation);
            return null;
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<IAnnotation>> GetAnnotations()
        {
            try
            {
                await LRSService.PersistQueuedStatements().ConfigureAwait(false);
            }
            catch (SendStatementException e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }

            return await drupalAPI.GetData<List<Annotation>>("api/annotations").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task JoinGroup(IGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            await drupalAPI.Post($"group/{group.Gid}/membership", requestBody: string.Empty).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task LeaveGroup(IGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            await drupalAPI.Delete($"group/{group.Gid}/membership").ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override async Task UpdateState(IEnumerable<IItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            try
            {
                await LRSService.PersistQueuedStatements().ConfigureAwait(false);
            }
            catch (SendStatementException e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }

            var nodes = items.OfType<Node>();
            var courseObjects = nodes.OfType<Course>().SelectMany(course => course.LearningObjects);
            var allNodes = nodes.Concat(courseObjects);

            var result = await drupalAPI.GetItemStates(allNodes).ConfigureAwait(false);

            if (result.ItemStates != null)
            {
                var lookup = new LearnerStateLookup(result.ItemStates);
                StateManager.BatchUpdateItemState(allNodes, lookup);
            }
        }

        /// <inheritdoc />
        protected override async Task UpdateState(IEnumerable<ITaxonomyTerm> terms)
        {
            if (terms == null)
            {
                throw new ArgumentNullException(nameof(terms));
            }

            try
            {
                await LRSService.PersistQueuedStatements().ConfigureAwait(false);
            }
            catch (SendStatementException e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }

            var allNodes = terms.OfType<TaxonomyTerm>();
            var result = await drupalAPI.GetTermStates(terms).ConfigureAwait(false);

            if (result.TermStates is IEnumerable<TermState> termStates)
            {
                var lookup = new LearnerTermStateLookup(termStates);
                TermStateManager.BatchUpdateTermState(allNodes, lookup);
            }
        }

        /// <inheritdoc />
        /// <remarks>This needs a cooresponding API endpoint to be useful.</remarks>
        protected override Task UpdateState(IItem item)
        {
            return UpdateState(new List<IItem> { item });
        }

        /// <inheritdoc />
        /// <remarks>This needs a cooresponding API endpoint to be useful.</remarks>
        protected override Task UpdateState(ITaxonomyTerm term)
        {
            return UpdateState(new List<ITaxonomyTerm> { term });
        }

        /// <inheritdoc />
        protected override void OnItemBookmarked(ILearner learner, IItem item)
        {
            base.OnItemBookmarked(learner, item);
            bookmarks.Insert(0, item);
            HandleBookmarksChanged();
        }

        /// <inheritdoc />
        protected override void OnItemUnbookmarked(ILearner learner, IItem item)
        {
            base.OnItemUnbookmarked(learner, item);

            var bookmarkedItem = bookmarks.FirstOrDefault((i) => i.Url == item.Url);
            if (bookmarkedItem == null)
            {
                return;
            }

            bookmarks.Remove(bookmarkedItem);
            HandleBookmarksChanged();
        }

        /// <inheritdoc />
        protected override void OnItemViewed(ILearner learner, IItem item)
        {
            base.OnItemViewed(learner, item);

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // Remove any instances of this item with the same ID.
            // Iterate over the list backwards so we can safely remove
            // items by index without causing a concurrent modification exception.
            for (int i = history.Count - 1; i >= 0; i--)
            {
                if (history[i].Id.Equals(item.Id))
                {
                    history.RemoveAt(i);
                }
            }

            history.Insert(0, item);
            HandleHistoryChanged();
        }

        async Task<IEnumerable<IItem>> PersistAndGetLearnerState(Task<IEnumerable<IItem>> retrieveStateTask, bool ignoreOfflineExceptions)
        {
            try
            {
                await LRSService.PersistQueuedStatements().ConfigureAwait(false);
                return await retrieveStateTask.ConfigureAwait(false);
            }
            catch (SendStatementException e) when (e.InnerException?.IsOfflineException() == true)
            {
                if (!ignoreOfflineExceptions)
                {
                    throw e.InnerException;
                }
            }
            catch (Exception e) when (e.IsOfflineException() && ignoreOfflineExceptions)
            {
            }
            catch (SendStatementException e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }

            return null;
        }

        void HandleHistoryChanged()
        {
            PersistentStore.Put(HistoryCacheKey, new List<IItem>(history));
        }

        void HandleBookmarksChanged()
        {
            PersistentStore.Put(BookmarksCacheKey, new List<IItem>(bookmarks));
        }

        class LearnerStateLookup : ILearnerStateLookup
        {
            readonly ILookup<int, ItemState> states;

            internal LearnerStateLookup(IEnumerable<ItemState> states)
            {
                this.states = states.ToLookup(state => state.Nid);
            }

            public CorpusItemLearnerState.Status GetBookmarkStatus(IItem item)
            {
                return GetStatus(item as Node, "bookmark");
            }

            public CorpusItemLearnerState.Status GetCompletionStatus(IItem item)
            {
                if (item is ITest)
                {
                    // While tests can have a completed flag,
                    // they also have a notion of "attempts".
                    // For tests that have at least one attempt,
                    // we will base the completion status off of that attempt.
                    var state = GetStateData(item as Node, "test_results");
                    if (state != null)
                    {
                        return GetStatus(state.LatestAttempt.IsComplete);
                    }
                }

                return GetStatus(item as Node, "completed");
            }

            public string GetRecommendationReason(IItem item)
            {
                return GetStateData(item as Node, "recommendation")?.RecommendationReason ?? string.Empty;
            }

            public string GetFeedback(IItem item)
            {
                if (item is not ITest)
                {
                    return string.Empty;
                }

                var state = GetStateData(item as Node, "test_results");
                return state?.LatestAttempt.Feedback ?? string.Empty;
            }

            CorpusItemLearnerState.Status GetStatus(Node node, string type)
            {
                if (node == null)
                {
                    return CorpusItemLearnerState.Status.Disabled;
                }

                return GetStatus(states[node.Nid]?.Any(state => state.Type == type));
            }

            ItemState GetStateData(Node node, string type)
            {
                if (node == null)
                {
                    return null;
                }

                return states[node.Nid]?.FirstOrDefault(state => state.Type == type);
            }

            CorpusItemLearnerState.Status GetStatus(bool? value)
            {
                if (value == true)
                {
                    return CorpusItemLearnerState.Status.Enabled;
                }

                return CorpusItemLearnerState.Status.Disabled;
            }
        }

        class LearnerTermStateLookup : ILearnerTermStateLookup
        {
            readonly ILookup<int, TermState> states;

            internal LearnerTermStateLookup(IEnumerable<TermState> states)
            {
                this.states = states.ToLookup(state => state.Tid);
            }

            public TaxonomyTermLearnerState.Status GetFollowingStatus(ITaxonomyTerm term)
            {
                return GetStatus(term, "following");
            }

            TaxonomyTermLearnerState.Status GetStatus(ITaxonomyTerm term, string type)
            {
                if (term == null)
                {
                    return TaxonomyTermLearnerState.Status.Disabled;
                }

                return GetStatus(states[term.Tid]?.Any(state => state.Type == type));
            }

            TaxonomyTermLearnerState.Status GetStatus(bool? value)
            {
                return value == true ? TaxonomyTermLearnerState.Status.Enabled : TaxonomyTermLearnerState.Status.Disabled;
            }
        }
    }
}
