using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PERLS.Data.Commands
{
    /// <summary>
    /// Similar to <see cref="Command{T}"/>, but adds an async method.
    /// </summary>
    /// <typeparam name="TIn">The type to provide to the execute function.</typeparam>
    public class AsyncCommand<TIn> : IAsyncCommand<TIn>
    {
        readonly Func<TIn, Task> task;
        readonly Command<Exception> exceptionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}"/> class.
        /// </summary>
        /// <param name="task">A function that takes a parameter and returns a task.</param>
        /// <param name="exceptionHandler">A handler for exceptions thrown during execution of the given task. Optional but recommended.</param>
        public AsyncCommand(Func<TIn, Task> task, Command<Exception> exceptionHandler = null)
        {
            this.task = task;
            this.exceptionHandler = exceptionHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}"/> class.
        /// </summary>
        /// <param name="task">A function that takes a parameter and returns a task.</param>
        /// <param name="exceptionHandler">A handler for exceptions thrown during execution of the given task. Optional but recommended.</param>
        public AsyncCommand(Func<Task> task, Command<Exception> exceptionHandler = null) : this(_ => task(), exceptionHandler)
        {
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        /// <inheritdoc />
        public bool CanExecute(TIn parameter)
        {
            return true;
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return parameter is TIn;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(TIn parameter)
        {
            try
            {
                await task(parameter).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                exceptionHandler?.Execute(e);
            }
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(object parameter)
        {
            if (parameter is TIn param)
            {
                await ExecuteAsync(param).ConfigureAwait(false);
            }
            else
            {
                exceptionHandler?.Execute(new InvalidCastException($"Cannot cast {parameter} to {typeof(TIn)}"));
            }
        }

        /// <inheritdoc />
        public void Execute(TIn parameter)
        {
            ExecuteAsync(parameter).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            ExecuteAsync(parameter).ConfigureAwait(false);
        }
    }
}
