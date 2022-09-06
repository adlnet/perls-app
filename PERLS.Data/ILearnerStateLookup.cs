using PERLS.Data.Definition;

namespace PERLS.Data
{
    /// <summary>
    /// Allows for batch updating of learner state by providing methods for looking up individual states by item.
    /// </summary>
    public interface ILearnerStateLookup
    {
        /// <summary>
        /// Gets the bookmark status for the specified item.
        /// </summary>
        /// <param name="item">The item whose state is being updated.</param>
        /// <returns>The bookmark status.</returns>
        CorpusItemLearnerState.Status GetBookmarkStatus(IItem item);

        /// <summary>
        /// Gets the completion status for the specified item.
        /// </summary>
        /// <param name="item">The item whose state is being updated.</param>
        /// <returns>The completion status.</returns>
        CorpusItemLearnerState.Status GetCompletionStatus(IItem item);

        /// <summary>
        /// Gets the recommendation reason for the specified item.
        /// </summary>
        /// <param name="item">The item whose state is being updated.</param>
        /// <returns>The recommendation reason.</returns>
        string GetRecommendationReason(IItem item);

        /// <summary>
        /// Gets the feedback for the specified item.
        /// </summary>
        /// <param name="item">The item whose state is being updated.</param>
        /// <returns>The feedback.</returns>
        string GetFeedback(IItem item);
    }
}
