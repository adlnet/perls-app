using System;
using System.Threading.Tasks;

namespace PERLS.Updates
{
    /// <summary>
    /// Interface which updates for a particular version update should follow.
    /// </summary>
    public interface IVersionUpdate
    {
        /// <summary>
        /// Tracks the update to ensure it has been ran.
        /// </summary>
        /// <returns>The name of the update.</returns>
        string GetUpdateName();

        /// <summary>
        /// Runs the update.
        /// </summary>
        /// <returns>Returns a task which contains the update to be ran.</returns>
        Task Update();
    }
}
