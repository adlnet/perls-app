using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Provides learner-specific content states.
    /// </summary>
    public interface ILearnerStateProvider
    {
        /// <summary>
        /// Retrieve the locally stored learner state for an item.
        /// </summary>
        /// <remarks>
        /// This should always return a <see cref="CorpusItemLearnerState"/> object
        /// with whatever information is immediately available in memory.
        /// </remarks>
        /// <param name="item">The corpus item.</param>
        /// <returns>The current learner state on that item.</returns>
        CorpusItemLearnerState GetState(IItem item);

        /// <summary>
        /// Retrieve the locally stored learner state for a term.
        /// </summary>
        /// <remarks>
        /// This should always return a <see cref="TaxonomyTermLearnerState"/> object
        /// with whatever information is immediately available in memory.
        /// </remarks>
        /// <param name="term">The corpus item.</param>
        /// <returns>The current learner state on that term.</returns>
        TaxonomyTermLearnerState GetState(ITaxonomyTerm term);

        /// <summary>
        /// Gets the states of all items requested. A call is also made to the server to get the current state of all the items that were included in the parameters.
        /// </summary>
        /// <returns>The state.</returns>
        /// <param name="items">Items.</param>
        Task<IEnumerable<CorpusItemLearnerState>> GetStateList(IEnumerable<IItem> items);

        /// <summary>
        /// Gets the states of all terms requested. A call is also made to the server to get the current state of all the terms that were included in the parameters.
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <returns>The state.</returns>
        Task<IEnumerable<TaxonomyTermLearnerState>> GetStateList(IEnumerable<ITaxonomyTerm> terms);

        /// <summary>
        /// Clears the learner state cache (useful for when the current learner changes).
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task ClearStateCache();

        /// <summary>
        /// Gets a list of bookmarks created by the current user.
        /// </summary>
        /// <returns>A list of bookmarks.</returns>
        Task<IEnumerable<IItem>> GetBookmarks();

        /// <summary>
        /// Toggles the bookmark status on the specified corpus item.
        /// </summary>
        /// <param name="item">The corpus item.</param>
        /// <returns>Asynchronous task.</returns>
        Task ToggleBookmark(IItem item);

        /// <summary>
        /// Toggles the following status on the specified term.
        /// </summary>
        /// <param name="term">The taxonomy term.</param>
        /// <returns>An awaitable task.</returns>
        Task ToggleFollowing(ITaxonomyTerm term);

        /// <summary>
        /// Gets a list of recently viewed items by the current user.
        /// </summary>
        /// <returns>A list of recently viewed items.</returns>
        Task<IEnumerable<IItem>> GetHistory();

        /// <summary>
        /// Gets the annotations.
        /// </summary>
        /// <returns>The annotations.</returns>
        Task<IEnumerable<IAnnotation>> GetAnnotations();

        /// <summary>
        /// Deletes an annotation.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="annotation">The annotation to be deleted.</param>
        Task DeleteAnnotation(IAnnotation annotation);

        /// <summary>
        /// Adds an item to the user's history.
        /// </summary>
        /// <param name="item">The corpus item.</param>
        /// <returns>An awaitable task.</returns>
        Task AddToHistory(IItem item);

        /// <summary>
        /// Marks an item as complete for the learner.
        /// </summary>
        /// <param name="item">The corpus item.</param>
        /// <returns>An awaitable task.</returns>
        Task MarkAsComplete(IItem item);

        /// <summary>
        /// Reports a learner's test result to the server.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="correctResponses">The number of correct responses.</param>
        /// <param name="duration">The test duration.</param>
        /// <returns>An awaitable task.</returns>
        Task SaveTestResult(ITest test, uint correctResponses, TimeSpan duration);

        /// <summary>
        /// Joins a specific group.
        /// </summary>
        /// <param name="group">The group to join.</param>
        /// <returns>An awaitable task.</returns>
        Task JoinGroup(IGroup group);

        /// <summary>
        /// Leave a specific group.
        /// </summary>
        /// <param name="group">The group to leave.</param>
        /// <returns>An awaitable task.</returns>
        Task LeaveGroup(IGroup group);
    }
}
