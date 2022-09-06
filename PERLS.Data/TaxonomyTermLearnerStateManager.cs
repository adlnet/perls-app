using System;
using System.Collections.Generic;
using System.Linq;
using Float.Core.Extensions;
using PERLS.Data.Definition;

namespace PERLS.Data
{
    /// <summary>
    /// Manages the state of multiple terms for the current learner.
    /// </summary>
    public class TaxonomyTermLearnerStateManager
    {
        const string PersistentStoreKey = "state_terms";
        static readonly object CacheLock = new object();
        readonly Dictionary<int, TaxonomyTermLearnerState> termCache;
        readonly ICacheStorage persistentStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxonomyTermLearnerStateManager"/> class.
        /// </summary>
        /// <param name="persistentStore">The persistent store to use to maintain the state of this class.</param>
        public TaxonomyTermLearnerStateManager(ICacheStorage persistentStore)
        {
            this.persistentStore = persistentStore ?? throw new ArgumentNullException(nameof(persistentStore));

            if (persistentStore.ContainsKey(PersistentStoreKey))
            {
                termCache = persistentStore.Get<Dictionary<int, TaxonomyTermLearnerState>>(PersistentStoreKey);
            }
            else
            {
                termCache = new Dictionary<int, TaxonomyTermLearnerState>();
            }
        }

        /// <summary>
        /// Performs a batch update of the learner state on the specified terms.
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <param name="stateLookup">The lookup for the current learner state.</param>
        public void BatchUpdateTermState(IEnumerable<ITaxonomyTerm> terms, ILearnerTermStateLookup stateLookup)
        {
            if (terms == null)
            {
                throw new ArgumentNullException(nameof(terms));
            }

            if (stateLookup == null)
            {
                throw new ArgumentNullException(nameof(stateLookup));
            }

            if (!terms.Any())
            {
                return;
            }

            terms.ForEach(term => GetState(term).Following = stateLookup.GetFollowingStatus(term));
            PersistStateChanges();
        }

        /// <summary>
        /// Persists the given status for the provided term.
        /// </summary>
        /// <param name="term">The term whose state should be persisted.</param>
        /// <param name="status">The status of the term to persist.</param>
        public void UpdateFollowingStatus(ITaxonomyTerm term, TaxonomyTermLearnerState.Status status)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            GetState(term).Following = status;
            PersistStateChanges();
        }

        /// <summary>
        /// Resets the cached state on all items.
        /// </summary>
        public void ClearCache()
        {
            foreach (var value in termCache.Values)
            {
                value.Reset();
            }

            PersistStateChanges();
        }

        internal TaxonomyTermLearnerState GetState(ITaxonomyTerm term)
        {
            lock (CacheLock)
            {
                return termCache.TryGetValue(term.Tid, out var state)
                    ? state
                    : (termCache[term.Tid] = new TaxonomyTermLearnerState(term));
            }
        }

        internal IEnumerable<TaxonomyTermLearnerState> GetState(IEnumerable<ITaxonomyTerm> terms)
        {
            return terms.Select(term => GetState(term));
        }

        void PersistStateChanges()
        {
            persistentStore.Put(PersistentStoreKey, termCache);
        }
    }
}
