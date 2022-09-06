using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Analytics;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using TinCan;
using TinCan.LRSResponses;
using Xamarin.Forms;

namespace PERLS.Data.ExperienceAPI
{
    /// <summary>
    /// Base implementation of a state provider that uses xAPI.
    /// </summary>
    public abstract class BaseExperienceAPILearnerStateProvider : ILearnerStateProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseExperienceAPILearnerStateProvider"/> class.
        /// </summary>
        /// <param name="persistentStore">The persistent store to use to maintain the state of this class.</param>
        protected BaseExperienceAPILearnerStateProvider(ICacheStorage persistentStore)
        {
            PersistentStore = persistentStore;
            StateManager = new CorpusItemLearnerStateManager(persistentStore);
            TermStateManager = new TaxonomyTermLearnerStateManager(persistentStore);
            LRSService = DependencyService.Get<ILRSService>();
            ReportingService = DependencyService.Get<IReportingService>();
        }

        /// <summary>
        /// Gets the current LRS service.
        /// </summary>
        /// <value>The LRS service.</value>
        protected ILRSService LRSService { get; }

        /// <summary>
        /// Gets the persistent store.
        /// </summary>
        /// <value>The persistent store.</value>
        protected ICacheStorage PersistentStore { get; }

        /// <summary>
        /// Gets the state manager.
        /// </summary>
        /// <value>The state manager.</value>
        protected CorpusItemLearnerStateManager StateManager { get; }

        /// <summary>
        /// Gets the term state manager.
        /// </summary>
        /// <value>The term state manager.</value>
        protected TaxonomyTermLearnerStateManager TermStateManager { get; }

        /// <summary>
        /// Gets the current xAPI reporting service.
        /// </summary>
        /// <value>The xAPI reporting service.</value>
        protected IReportingService ReportingService { get; }

        /// <inheritdoc />
        public abstract Task<IEnumerable<IItem>> GetBookmarks();

        /// <inheritdoc />
        public abstract Task<IEnumerable<IItem>> GetHistory();

        /// <inheritdoc/>
        public abstract Task DeleteAnnotation(IAnnotation annotation);

        /// <inheritdoc/>
        public abstract Task<IEnumerable<IAnnotation>> GetAnnotations();

        /// <inheritdoc/>
        public abstract Task JoinGroup(IGroup group);

        /// <inheritdoc/>
        public abstract Task LeaveGroup(IGroup group);

        /// <inheritdoc />
        public virtual CorpusItemLearnerState GetState(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var state = StateManager.GetState(item);
            if (state.IsUnknown)
            {
                state.MarkAsRetrievingUpdatedState();
                UpdateState(item);
            }

            return state;
        }

        /// <inheritdoc />
        public virtual TaxonomyTermLearnerState GetState(ITaxonomyTerm term)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            var state = TermStateManager.GetState(term);

            if (state.IsUnknown)
            {
                state.MarkAsRetrievingUpdatedState();
                UpdateState(term);
            }

            return state;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns>The state.</returns>
        /// <param name="items">Items.</param>
        public virtual async Task<IEnumerable<CorpusItemLearnerState>> GetStateList(IEnumerable<IItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            await UpdateState(items).ConfigureAwait(false);
            return StateManager.GetState(items);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TaxonomyTermLearnerState>> GetStateList(IEnumerable<ITaxonomyTerm> terms)
        {
            if (terms == null)
            {
                throw new ArgumentNullException(nameof(terms));
            }

            await UpdateState(terms).ConfigureAwait(false);
            return TermStateManager.GetState(terms);
        }

        /// <inheritdoc />
        public Task ClearStateCache()
        {
            StateManager.ClearCache();
            TermStateManager.ClearCache();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual async Task ToggleBookmark(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (DependencyService.Get<IAppContextService>().CurrentLearner is not ILearner learner)
            {
                throw new InvalidOperationException(Strings.NoLearnerError);
            }

            var state = StateManager.GetState(item);

            if (state.Bookmarked == CorpusItemLearnerState.Status.Retrieving)
            {
                return;
            }

            var isBookmarked = state.Bookmarked == CorpusItemLearnerState.Status.Enabled;

            state.Bookmarked = CorpusItemLearnerState.Status.Retrieving;

            try
            {
                if (isBookmarked)
                {
                    await UnbookmarkItem(learner, item).ConfigureAwait(false);
                    state.Bookmarked = CorpusItemLearnerState.Status.Disabled;
                }
                else
                {
                    await BookmarkItem(learner, item).ConfigureAwait(false);
                    state.Bookmarked = CorpusItemLearnerState.Status.Enabled;
                }
            }
            catch (Exception e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
                state.Bookmarked = CorpusItemLearnerState.Status.Unknown;
                throw;
            }
        }

        /// <inheritdoc />
        public virtual Task ToggleFollowing(ITaxonomyTerm term)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            if (DependencyService.Get<IAppContextService>().CurrentLearner is not ILearner learner)
            {
                throw new InvalidOperationException(Strings.NoLearnerError);
            }

            return UpdateFollowStatus(term, learner);
        }

        /// <inheritdoc />
        public virtual Task AddToHistory(IItem item)
        {
            if (DependencyService.Get<IAppContextService>().CurrentLearner is not ILearner learner)
            {
                throw new InvalidOperationException(Strings.NoLearnerError);
            }

            OnItemViewed(learner, item);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual async Task SaveTestResult(ITest test, uint correctResponses, TimeSpan duration)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            var totalQuestions = test.Questions.Count();
            var feedback = string.Format(CultureInfo.CurrentCulture, Strings.CorrectAnswersLabel, correctResponses, totalQuestions);

            var state = StateManager.GetState(test);
            StateManager.UpdateTestResults(test, feedback);

            ReportingService.ReportTestCompleted(test, correctResponses, duration);

            try
            {
                state.Completed = CorpusItemLearnerState.Status.Retrieving;
                await LRSService.PersistQueuedStatements().ConfigureAwait(false);

                // Get updated state information from the server.
                await UpdateState(test).ConfigureAwait(false);
            }
            catch
            {
                // Something went wrong getting the updated state of this item.
                // Just _assume_ that it is complete.
                state.Completed = CorpusItemLearnerState.Status.Enabled;
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task MarkAsComplete(IItem item)
        {
            if (DependencyService.Get<IAppContextService>().CurrentLearner is not ILearner learner)
            {
                throw new InvalidOperationException(Strings.NoLearnerError);
            }

            // Assume that if the content was packaged content
            // that it was sending it's own xAPI statements,
            // so we don't need to send anymore.
            if (item is not IPackagedContent)
            {
                var statement = StatementBuilder
                    .LearnerActor()
                    .SetVerb(Profiles.Cmi5.Verbs.Completed)
                    .SetObject(item)
                    .GetStatement();

                await SendStatement(statement).ConfigureAwait(false);
            }

            StateManager.UpdateCompletedStatus(item, CorpusItemLearnerState.Status.Enabled);
            OnItemCompleted(learner, item);
        }

        /// <summary>
        /// Invoked when an item has been bookmarked.
        /// </summary>
        /// <param name="learner">The current learner.</param>
        /// <param name="item">The item being bookmarked.</param>
        protected virtual void OnItemBookmarked(ILearner learner, IItem item)
        {
        }

        /// <summary>
        /// Invoked when an item has been removed from bookmarks.
        /// </summary>
        /// <param name="learner">The current learner.</param>
        /// <param name="item">The item being unbookmarked.</param>
        protected virtual void OnItemUnbookmarked(ILearner learner, IItem item)
        {
        }

        /// <summary>
        /// Invoked when a term has been followed.
        /// </summary>
        /// <param name="learner">The current learner.</param>
        /// <param name="term">The term being followed.</param>
        protected virtual void OnTermFollowed(ILearner learner, ITaxonomyTerm term)
        {
        }

        /// <summary>
        /// Invoked when a term as been unfollowed.
        /// </summary>
        /// <param name="learner">The current learner.</param>
        /// <param name="term">The term being unfollowed.</param>
        protected virtual void OnTermUnfollowed(ILearner learner, ITaxonomyTerm term)
        {
        }

        /// <summary>
        /// Invoked when an item has been viewed.
        /// </summary>
        /// <param name="learner">The current learner.</param>
        /// <param name="item">The item being viewed.</param>
        protected virtual void OnItemViewed(ILearner learner, IItem item)
        {
        }

        /// <summary>
        /// Invoked when an item has been completed.
        /// </summary>
        /// <param name="learner">The current learner.</param>
        /// <param name="item">The item that was completed.</param>
        protected virtual void OnItemCompleted(ILearner learner, IItem item)
        {
        }

        /// <summary>
        /// Requested updated state information from the server.
        /// </summary>
        /// <param name="item">The cooresponding item.</param>
        /// <returns>The update task.</returns>
        protected abstract Task UpdateState(IItem item);

        /// <summary>
        /// Requested updated state information from the server.
        /// </summary>
        /// <param name="term">The corresponding term.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task UpdateState(ITaxonomyTerm term);

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <returns>The state.</returns>
        /// <param name="items">Items.</param>
        protected abstract Task UpdateState(IEnumerable<IItem> items);

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <param name="terms">Terms.</param>
        /// <returns>The state.</returns>
        protected abstract Task UpdateState(IEnumerable<ITaxonomyTerm> terms);

        async Task<StatementLRSResponse> SendStatement(Statement statement)
        {
            var response = await LRSService.SaveStatement(statement).ConfigureAwait(false);

            if (!response.success)
            {
                if (response.httpException != null)
                {
                    throw response.httpException;
                }

                throw new SendStatementException(response.errMsg ?? Strings.StatementSendError);
            }

            return response;
        }

        async Task BookmarkItem(ILearner learner, IItem item)
        {
            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Favorited)
                .SetObject(item)
                .GetStatement();

            await SendStatement(statement).ConfigureAwait(false);

            OnItemBookmarked(learner, item);
        }

        async Task UnbookmarkItem(ILearner learner, IItem item)
        {
            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Unfavorited)
                .SetObject(item)
                .GetStatement();

            await SendStatement(statement).ConfigureAwait(false);

            OnItemUnbookmarked(learner, item);
        }

        async Task FollowTerm(ILearner learner, ITaxonomyTerm term)
        {
            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Followed)
                .SetObject(term)
                .GetStatement();

            await SendStatement(statement).ConfigureAwait(false);

            OnTermFollowed(learner, term);
        }

        async Task UnfollowTerm(ILearner learner, ITaxonomyTerm term)
        {
            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Unfollowed)
                .SetObject(term)
                .GetStatement();

            await SendStatement(statement).ConfigureAwait(false);

            OnTermUnfollowed(learner, term);
        }

        async Task UpdateFollowStatus(ITaxonomyTerm term, ILearner learner)
        {
            var state = TermStateManager.GetState(term);
            var isFollowing = state.Following == TaxonomyTermLearnerState.Status.Enabled;
            state.Following = TaxonomyTermLearnerState.Status.Retrieving;

            try
            {
                if (isFollowing)
                {
                    await UnfollowTerm(learner, term).ConfigureAwait(false);
                    state.Following = TaxonomyTermLearnerState.Status.Disabled;
                }
                else
                {
                    await FollowTerm(learner, term).ConfigureAwait(false);
                    state.Following = TaxonomyTermLearnerState.Status.Enabled;
                }
            }
            catch (Exception e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
                state.Following = TaxonomyTermLearnerState.Status.Unknown;
                throw;
            }

            TermStateManager.UpdateFollowingStatus(term, state.Following);
        }
    }
}
