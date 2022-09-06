using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PERLS
{
    /// <summary>
    /// Executes an asynchronous action against a queue of items.
    /// </summary>
    /// <typeparam name="T">The type of object being processed.</typeparam>
    /// <remarks>
    /// By default, it will retry an item if it fails (i.e. the action throws an exception).
    /// </remarks>
    public class ActionQueue<T>
    {
        readonly IList<T> queue = new List<T>();
        readonly Func<T, Task> action;
        Task execution;
        bool isCancelling;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionQueue{T}"/> class.
        /// </summary>
        /// <param name="action">The action to perform on each item in the queue.</param>
        public ActionQueue(Func<T, Task> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <summary>
        /// Gets a value indicating whether the queue is currently running.
        /// </summary>
        /// <value><c>true</c> if the queue is currently processing items.</value>
        public bool IsRunning => execution?.IsCompleted == false;

        /// <summary>
        /// Gets or sets the number of seconds to wait before retrying an action.
        /// </summary>
        /// <value>The number of seconds to backoff on each attempt.</value>
        /// <remarks>
        /// When processing an item fails, the queue progressively backs-off making additional attempts.
        /// For example, if the interval was 5, the queue would wait 5 seconds after the first failure,
        /// 10 seconds after the second failure, and 15 seconds after the third (and so on).
        /// </remarks>
        public uint BackoffInterval { get; set; } = 30; // seconds

        /// <summary>
        /// Gets or sets the maximum number of tries for each item in the queue.
        /// </summary>
        /// <value>The maximum number of retries.</value>
        public uint MaxRetries { get; set; } = 5;

        /// <summary>
        /// Gets or sets the item that last failed processing.
        /// </summary>
        /// <value>The item that failed processing.</value>
        public T LastFailure { get; protected set; }

        /// <summary>
        /// Adds a new item to the queue and implicitly starts the queue.
        /// </summary>
        /// <param name="item">The item to add to the queue.</param>
        /// <remarks>
        /// An item cannot be added more than once.
        /// </remarks>
        public void Enqueue(T item)
        {
            if (queue.Contains(item) || isCancelling)
            {
                return;
            }

            queue.Add(item);
            Start();
        }

        /// <summary>
        /// Cancels a queued item.
        /// </summary>
        /// <param name="item">The item to remove from the queue.</param>
        /// <returns><c>true</c> if the item was removed from the queue.</returns>
        /// <remarks>
        /// An item cannot be removed from the queue if it is currently
        /// being processed.
        /// </remarks>
        public bool Cancel(T item)
        {
            if (item is IEquatable<T> equatable && equatable.Equals(queue.FirstOrDefault()))
            {
                return false;
            }

            return queue.Remove(item);
        }

        /// <summary>
        /// Cancels a queued item matching a predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns><c>true</c> if the item was removed from the queue.</returns>
        /// <remarks>
        /// An item cannot be removed from the queue if it is currently
        /// being processed.
        /// </remarks>
        public bool Cancel(Func<T, bool> predicate)
        {
            if (queue.FirstOrDefault(predicate) is T item)
            {
                Cancel(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Stops the queue.
        /// </summary>
        /// <remarks>
        /// The queue will resume when another item is added
        /// or by calling <see cref="Start"/>.
        /// </remarks>
        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            isCancelling = true;
        }

        /// <summary>
        /// Clears and stops the queue.
        /// </summary>
        public void Clear()
        {
            isCancelling = IsRunning;
            queue.Clear();
            LastFailure = default;
        }

        /// <summary>
        /// Starts the queue.
        /// </summary>
        /// <returns>A task representing the entire queue.</returns>
        /// <remarks>
        /// The queue starts automatically when a task is enqueued.
        /// However, you can call this to resume a paused queue or to get
        /// reference to the current task execution.
        /// </remarks>
        public virtual Task Start()
        {
            if (!IsRunning && queue.Any())
            {
                execution = Execute();
            }

            return execution ?? Task.CompletedTask;
        }

        /// <summary>
        /// Begins processing the next item in the queue.
        /// </summary>
        /// <returns>A task representing the processing of the item.</returns>
        /// <remarks>
        /// The item is always removed from the queue when this method returns.
        /// If it failed, it will be stored in <see cref="LastFailure"/>.
        /// </remarks>
        protected virtual async Task Next()
        {
            var item = queue.FirstOrDefault();
            if (item == null)
            {
                return;
            }

            try
            {
                await AttemptToProcess(item).ConfigureAwait(false);
            }
            catch
            {
                LastFailure = item;
                throw;
            }
            finally
            {
                queue.Remove(item);
            }
        }

        /// <summary>
        /// Processes an item.
        /// </summary>
        /// <param name="item">The item to process.</param>
        /// <returns>A task representing the processing of the item.</returns>
        protected virtual Task Process(T item)
        {
            return action(item);
        }

        async Task Execute()
        {
            while (queue.Any() && !isCancelling)
            {
                try
                {
                    await Next().ConfigureAwait(false);
                }
                catch
                {
                    // Ignore all exceptions.
                    // The action passed in should be handling exceptions.
                    // The queue must go on.
                }
            }

            isCancelling = false;
        }

        async Task AttemptToProcess(T item)
        {
            var tries = 0;

            do
            {
                try
                {
                    await Process(item);
                    return;
                }
                catch (Exception) when (!queue.Contains(item))
                {
                    // The item is no longer in the queue so it should not be retried.
                    // ActionQueue doesn't remove an item until it's successful so
                    // this means some other process removed it.
                    // This will not be considered a failure.
                    return;
                }
                catch (Exception) when (++tries < MaxRetries)
                {
                    await Task.Delay(tries * (int)BackoffInterval * 1000);
                }
            }
            while (!isCancelling);
        }
    }
}
