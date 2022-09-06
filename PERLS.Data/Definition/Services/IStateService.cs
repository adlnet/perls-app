using System.Threading.Tasks;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// Defines a service for managing learner state.
    /// </summary>
    public interface IStateService
    {
        /// <summary>
        /// Saves a the position.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position.</param>
        void SavePosition(IItem item, double position);

        /// <summary>
        /// Retrieves the position.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<double> RetrievePosition(IItem item);

        /// <summary>
        /// Deletes the position.
        /// </summary>
        /// <param name="item">The item.</param>
        void DeletePosition(IItem item);
    }
}
