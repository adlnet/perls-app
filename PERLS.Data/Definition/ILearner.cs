using Float.Core.Definitions;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// A learner.
    /// </summary>
    public interface ILearner : INamed, IRemoteResource
    {
        /// <summary>
        /// Gets the learner's email address.
        /// </summary>
        /// <value>The learner email address.</value>
        string Email { get; }

        /// <summary>
        /// Gets a URI for the learner's avatar.
        /// </summary>
        /// <value>A learner avatar URI.</value>
        IFile Avatar { get; }

        /// <summary>
        /// Gets the path to edit the user's profile.
        /// </summary>
        /// <value>The edit URL.</value>
        string EditPath { get; }

        /// <summary>
        /// Gets the Learner Goals.
        /// </summary>
        /// <value>
        /// The Learner Goals.
        /// </value>
        ILearnerGoals LearnerGoals { get; }

        /// <summary>
        /// Gets the learner's preferred language.
        /// </summary>
        /// <value>
        /// The learner's preferred language.
        /// </value>
        string PreferredLanguage { get; }
    }
}
