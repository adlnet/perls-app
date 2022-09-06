using System;
using System.Threading.Tasks;
using TinCan;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// A service to provide a configurable LRS endpoint.
    /// </summary>
    public interface ILRSService : ILRS
    {
        /// <summary>
        /// Sets the current LRS endpoint.
        /// </summary>
        /// <param name="uri">The URI of the LRS.</param>
        void UpdateEndpoint(Uri uri);

        /// <summary>
        /// Clears any cached or queued data.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns only the locally cached statements.
        /// </summary>
        /// <returns>A string representing cached statements.</returns>
        string RawLocalCache();

        /// <summary>
        /// Persist all the queued statements in the statement queue.
        /// </summary>
        /// <remarks>
        /// The entire statement queue should be flushed before trying to retrieve
        /// an updated learner state from the server. The queued xAPI statements
        /// could represent updates to the learner's state.
        /// Throws an exception if the queue is not empty after flushing.
        /// </remarks>
        /// <returns>An awaitable task.</returns>
        Task PersistQueuedStatements();

        /// <summary>
        /// Persist the queued state in the state queue.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task PersistQueuedState();
    }
}
