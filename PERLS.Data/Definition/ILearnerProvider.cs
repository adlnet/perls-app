using System.Collections.Generic;
using System.Threading.Tasks;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Retrieves learner information.
    /// </summary>
    /// <remarks>
    /// Consider: retrieving _current_ learner may be a different service than retrieving learner information in general.
    /// </remarks>
    public interface ILearnerProvider
    {
        /// <summary>
        /// Gets the current learner.
        /// </summary>
        /// <returns>The current learner.</returns>
        Task<ILearner> GetCurrentLearner();

        /// <summary>
        /// Retrieves usage stats for the current learner.
        /// </summary>
        /// <returns>Usage stats for the current learner.</returns>
        Task<ILearnerStats> GetCurrentLearnerStats();

        /// <summary>
        /// Saves the current learner goals.
        /// </summary>
        /// <returns>The learner goals.</returns>
        Task SaveCurrentLearnerGoals();

        /// <summary>
        /// Retrieves the prompts for the current learner.
        /// </summary>
        /// <returns>The task for getting the prompts.</returns>
        Task<IEnumerable<IPrompt>> GetPrompts();

        /// <summary>
        /// Retrieves the certificates for the current learner.
        /// </summary>
        /// <returns>The task for getting the certificates.</returns>
        Task<IEnumerable<ICertificate>> GetCertificates();

        /// <summary>
        /// Retrieves the item from a unique identifier.
        /// </summary>
        /// <returns>The item associated with the unique identifier, or null if none was found.</returns>
        /// <param name="itemId">The unique item identifier.</param>
        Task<ICertificate> GetCertificateItemFromId(string itemId);

        /// <summary>
        /// Retrieves the badges.
        /// </summary>
        /// <returns>The badges.</returns>
        Task<IEnumerable<IBadge>> GetBadges();

        /// <summary>
        /// Retrieves the item from a unique identifier.
        /// </summary>
        /// <returns>The item associated with the unique identifier, or null if none was found.</returns>
        /// <param name="itemId">The unique item identifier.</param>
        Task<IBadge> GetBadgeItemFromId(string itemId);
    }
}
