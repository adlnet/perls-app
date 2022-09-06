using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PERLS.Data.Commands
{
    /// <summary>
    /// Similar to <see cref="Command"/>, but adds an async method.
    /// </summary>
    public class AsyncCommand : IAsyncCommand
    {
        readonly Func<object, Task> task;
        readonly Command<Exception> exceptionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="task">A function that takes a parameter and returns a task.</param>
        /// <param name="exceptionHandler">A handler for exceptions thrown during exceution of the given task. Optional but recommended.</param>
        public AsyncCommand(Func<object, Task> task, Command<Exception> exceptionHandler = null)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.exceptionHandler = exceptionHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="task">A function that returns a task.</param>
        /// <param name="exceptionHandler">A handler for exceptions thrown during exceution of the given task. Optional but recommended.</param>
        public AsyncCommand(Func<Task> task, Command<Exception> exceptionHandler = null)
        {
            this.task = _ => task();
            this.exceptionHandler = exceptionHandler;
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(object parameter)
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
        public void Execute(object parameter)
        {
            ExecuteAsync(parameter).ConfigureAwait(false);
        }
    }
}
