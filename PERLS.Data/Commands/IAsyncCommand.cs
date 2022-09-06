using System.Threading.Tasks;
using System.Windows.Input;

namespace PERLS.Data.Commands
{
    /// <summary>
    /// Extends a command with an async execution ability.
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Execute this command asynchronously.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>An awaitable task.</returns>
        Task ExecuteAsync(object parameter);
    }

    /// <summary>
    /// Extends a command with an async execution ability.
    /// </summary>
    /// <typeparam name="TIn">The type to provide to the execute function.</typeparam>
    public interface IAsyncCommand<TIn> : ICommand<TIn>, IAsyncCommand
    {
        /// <summary>
        /// Execute this command asynchronously.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>An awaitable task.</returns>
        Task ExecuteAsync(TIn parameter);
    }

    /// <summary>
    /// Extends a command with an async execution ability.
    /// </summary>
    /// <typeparam name="TIn">The type to provide to the execute function.</typeparam>
    /// <typeparam name="TOut">The type returned from the task.</typeparam>
    public interface IAsyncCommand<TIn, TOut> : ICommand<TIn>, IAsyncCommand
    {
        /// <summary>
        /// Execute this command asynchronously.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>An awaitable task.</returns>
        Task<TOut> ExecuteAsync(TIn parameter);
    }

    /// <summary>
    /// Extends a command with an async execution ability.
    /// </summary>
    /// <typeparam name="TOut">The type to provided by the execute function.</typeparam>
    public interface IAsyncOutCommand<TOut> : IAsyncCommand<object, TOut>
    {
    }
}
