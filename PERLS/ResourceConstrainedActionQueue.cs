using System;
using System.Threading.Tasks;

namespace PERLS
{
    /// <summary>
    /// An action queue that considers available resources on the device
    /// and avoid performing an action when resources are not available.
    /// </summary>
    /// <typeparam name="T">The type of item being processed.</typeparam>
    /// <remarks>
    /// For example, this could be used to avoid performing an action while the device battery level is too low.
    /// Just as with a normal action queue, this will attempt to retry in a resource-constrained situation.
    /// However, if the number of retries exceeds <see cref="ActionQueue{T}.MaxRetries"/>, then the queue stops
    /// (it can be resumed with <see cref="ActionQueue{T}.Start"/>).
    /// </remarks>
    public class ResourceConstrainedActionQueue<T> : ActionQueue<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceConstrainedActionQueue{T}"/> class.
        /// </summary>
        /// <param name="action">The action to perform on each item in the queue.</param>
        public ResourceConstrainedActionQueue(Func<T, Task> action) : base(action)
        {
        }

        /// <summary>
        /// Checks whether there are enough resources to process the queue.
        /// </summary>
        /// <returns><c>true</c> if there are sufficient resources for processing.</returns>
        public delegate bool ResourceCheck();

        /// <summary>
        /// Gets or sets the delegate responsible for determining if there are enough resources for processing the queue.
        /// </summary>
        /// <value>The delegate responsible for checking resource availability.</value>
        public ResourceCheck HasSufficientResources { get; set; }

        /// <inheritdoc />
        protected override async Task Next()
        {
            try
            {
                await base.Next();
            }
            catch (ResourceConstrainedException)
            {
                // Put the item back in the queue and stop processing the queue.
                Enqueue(LastFailure);
                LastFailure = default;
                Stop();
                throw;
            }
        }

        /// <inheritdoc />
        protected override Task Process(T item)
        {
            if (HasSufficientResources != null && !HasSufficientResources())
            {
                throw new ResourceConstrainedException();
            }

            return base.Process(item);
        }

        class ResourceConstrainedException : Exception
        {
            internal ResourceConstrainedException() : base()
            {
            }
        }
    }
}
