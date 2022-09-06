using PERLS.Data.Definition;

namespace PERLS.Data
{
    /// <summary>
    /// Allows for batch updating of learner state by providing methods for looking up individual states by term.
    /// </summary>
    public interface ILearnerTermStateLookup
    {
        /// <summary>
        /// Gets the following status for the specified term.
        /// </summary>
        /// <param name="term">The term whose state is being updated.</param>
        /// <returns>The following status.</returns>
        TaxonomyTermLearnerState.Status GetFollowingStatus(ITaxonomyTerm term);
    }
}
