using System;
using System.Collections.Generic;
using System.Linq;
using Float.Core.Extensions;
using PERLS.Data.Definition;

namespace PERLS.Data
{
    /// <summary>
    /// Manages the state of multiple items for the current learner.
    /// </summary>
    public class CorpusItemLearnerStateManager
    {
        const string PersistentStoreKey = "state";
        static readonly object CacheLock = new object();
        readonly Dictionary<Guid, CorpusItemLearnerState> stateCache;
        readonly ICacheStorage persistentStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorpusItemLearnerStateManager"/> class.
        /// </summary>
        /// <param name="persistentStore">The persistent store to use to maintain the state of this class.</param>
        public CorpusItemLearnerStateManager(ICacheStorage persistentStore)
        {
            this.persistentStore = persistentStore ?? throw new ArgumentNullException(nameof(persistentStore));

            if (persistentStore.ContainsKey(PersistentStoreKey))
            {
                stateCache = persistentStore.Get<Dictionary<Guid, CorpusItemLearnerState>>(PersistentStoreKey);
            }
            else
            {
                stateCache = new Dictionary<Guid, CorpusItemLearnerState>();
            }
        }

        /// <summary>
        /// Performs a batch update of the learner state on the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="stateLookup">The lookup for the current learner state.</param>
        public void BatchUpdateItemState(IEnumerable<IItem> items, ILearnerStateLookup stateLookup)
        {
            if (!items.Any())
            {
                return;
            }

            items.ForEach(item =>
            {
                var state = GetState(item);
                state.Bookmarked = stateLookup.GetBookmarkStatus(item);
                state.Completed = stateLookup.GetCompletionStatus(item);
                state.RecommendationReason = stateLookup.GetRecommendationReason(item);
                state.Feedback = stateLookup.GetFeedback(item);
            });

            PersistStateChanges();
        }

        /// <summary>
        /// Updates the bookmarked status on the corpus item.
        /// </summary>
        /// <param name="item">The corpus item.</param>
        /// <param name="status">The new status.</param>
        public void UpdateBookmarkedStatus(IItem item, CorpusItemLearnerState.Status status)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            GetState(item).Bookmarked = status;
            PersistStateChanges();
        }

        /// <summary>
        /// Updates the completion status on the corpus item.
        /// </summary>
        /// <param name="item">The corpus item.</param>
        /// <param name="status">The new status.</param>
        public void UpdateCompletedStatus(IItem item, CorpusItemLearnerState.Status status)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            GetState(item).Completed = status;
            PersistStateChanges();
        }

        /// <summary>
        /// Updates the recommendation reason.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="reason">Reason.</param>
        public void UpdateRecommendationReason(IItem item, string reason)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            GetState(item).RecommendationReason = reason;
            PersistStateChanges();
        }

        /// <summary>
        /// Updates the cached list of bookmarked items in bulk.
        /// </summary>
        /// <param name="items">The new list of bookmarked items.</param>
        /// <param name="oldItems">Optionally, the old list of bookmarked items.</param>
        public void UpdateBookmarkedItems(IEnumerable<IItem> items, IEnumerable<IItem> oldItems = null)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (oldItems != null)
            {
                foreach (var item in oldItems)
                {
                    GetState(item).Bookmarked = CorpusItemLearnerState.Status.Disabled;
                }
            }

            foreach (var item in items)
            {
                GetState(item).Bookmarked = CorpusItemLearnerState.Status.Enabled;
            }

            PersistStateChanges();
        }

        /// <summary>
        /// Resets the cached state on all items.
        /// </summary>
        public void ClearCache()
        {
            lock (CacheLock)
            {
                foreach (var value in stateCache.Values)
                {
                    value.Reset();
                }
            }

            PersistStateChanges();
        }

        /// <summary>
        /// Update the test results for the specified test.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="feedback">The feedback to display to the learner.</param>
        internal void UpdateTestResults(ITest test, string feedback)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            var state = GetState(test);
            state.Feedback = feedback;
            PersistStateChanges();
        }

        /// <summary>
        /// Gets the cached state of a corpus item.
        /// </summary>
        /// <param name="item">The corpus item.</param>
        /// <returns>The cached state.</returns>
        internal CorpusItemLearnerState GetState(IItem item)
        {
            lock (CacheLock)
            {
                if (!stateCache.ContainsKey(item.Id))
                {
                    stateCache[item.Id] = new CorpusItemLearnerState(item);
                }

                return stateCache[item.Id];
            }
        }

        internal IEnumerable<CorpusItemLearnerState> GetState(IEnumerable<IItem> items)
        {
            List<CorpusItemLearnerState> corpusItemLearnerStates = new List<CorpusItemLearnerState>();

            foreach (var item in items)
            {
                var state = GetState(item);
                corpusItemLearnerStates.Add(state);
            }

            return corpusItemLearnerStates;
        }

        void PersistStateChanges()
        {
            persistentStore.Put(PersistentStoreKey, stateCache);
        }
    }
}
