using System.Windows.Input;

namespace PERLS.Data.Commands
{
    /// <summary>
    /// An interface for typed commands.
    /// </summary>
    /// <typeparam name="T">The type of parameter to provide to the execute function.</typeparam>
    public interface ICommand<T> : ICommand
    {
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        /// <returns><c>true</c> if this command can be executed, <c>false</c> otherwise.</returns>
        bool CanExecute(T parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        void Execute(T parameter);
    }

    /// <summary>
    /// An interface for typed commands with both input and output types.
    /// </summary>
    /// <typeparam name="TIn">The type of input to the <see cref="CanExecute(TIn)"/> and <see cref="Execute(TIn)"/> functions.</typeparam>
    /// <typeparam name="TOut">The type of output from the <see cref="Execute(TIn)"/> function.</typeparam>
    public interface ICommand<TIn, TOut> : ICommand
    {
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        /// <returns><c>true</c> if this command can be executed, <c>false</c> otherwise.</returns>
        bool CanExecute(TIn parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        /// <returns>The output type of the command.</returns>
        TOut Execute(TIn parameter);
    }
}
