using System;
using System.Threading.Tasks;

namespace PERLS.DataImplementation.Providers
{
    /// <summary>
    /// Contains a task that can be run repeatedly.
    /// Subsequent calls to <c>Run()</c> before the task finishes will await the same task.
    /// After the task completes, the next call to <c>Run()</c> will create a new task.
    /// </summary>
    /// <typeparam name="TResult">The type returned from the given factory method.</typeparam>
    public class RepeatableTaskContainer<TResult>
    {
        readonly Func<Task<TResult>> taskFactory;
        Task<TResult> runningTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatableTaskContainer{T}"/> class.
        /// </summary>
        /// <param name="taskFactory">A function to call asynchronously.</param>
        public RepeatableTaskContainer(Func<Task<TResult>> taskFactory)
        {
            this.taskFactory = taskFactory;
        }

        /// <summary>
        /// Run the given function asynchronously and return the result to awaiters.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<TResult> Run()
        {
            if (runningTask == null)
            {
                runningTask = taskFactory();
            }

            try
            {
                return await runningTask.ConfigureAwait(false);
            }
            finally
            {
                runningTask?.Dispose();
                runningTask = null;
            }
        }
    }
}
