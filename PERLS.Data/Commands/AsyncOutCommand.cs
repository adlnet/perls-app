using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PERLS.Data.Commands
{
    /// <summary>
    /// Similar to <see cref="AsyncCommand{T}"/>, but types the output instead of the input.
    /// </summary>
    /// <typeparam name="TOut">The type provided by the execute function.</typeparam>
    public class AsyncOutCommand<TOut> : IAsyncOutCommand<TOut>
    {
        readonly Func<object, Task<TOut>> task;
        readonly Command<Exception> exceptionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncOutCommand{T}"/> class.
        /// </summary>
        /// <param name="task">A function that takes a parameter and returns a task.</param>
        /// <param name="exceptionHandler">A handler for exceptions thrown during execution of the given task. Optional but recommended.</param>
        public AsyncOutCommand(Func<object, Task<TOut>> task, Command<Exception> exceptionHandler = null)
        {
            this.task = obj => task(obj);
            this.exceptionHandler = exceptionHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncOutCommand{T}"/> class.
        /// </summary>
        /// <param name="task">A function that takes no parameters and returns a task.</param>
        /// <param name="exceptionHandler">A handler for exceptions thrown during execution of the given task. Optional but recommended.</param>
        public AsyncOutCommand(Func<Task<TOut>> task, Command<Exception> exceptionHandler = null) : this(_ => task(), exceptionHandler)
        {
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged
        {
            // since this event is currently unused, we need these to avoid compiler warning CS0067
            add { }
            remove { }
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <inheritdoc />
        public async Task<TOut> ExecuteAsync(object parameter)
        {
            try
            {
                return await task(parameter).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                exceptionHandler?.Execute(e);
                return default;
            }
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            ExecuteAsync(parameter).ConfigureAwait(false);
        }

        /// <inheritdoc />
        async Task IAsyncCommand.ExecuteAsync(object parameter)
        {
            await ExecuteAsync(parameter).ConfigureAwait(false);
        }
    }
}
