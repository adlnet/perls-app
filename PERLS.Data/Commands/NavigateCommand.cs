using System;
using Float.Core.Commands;

namespace PERLS.Data.Commands
{
    /// <summary>
    /// A command used for handling a user's request to navigate to somewhere in the app.
    /// The command includes debouncing logic to prevent the user from mashing buttons.
    /// </summary>
    /// <typeparam name="T">An object representing the destination requested by the user.</typeparam>
    public class NavigateCommand<T> : DebounceCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The action to perform.</param>
        /// <param name="delay">The delay until another command can be executed (default is 300ms).</param>
        public NavigateCommand(Action<NavigationOption<T>> execute, int delay = 300) : base(new Xamarin.Forms.Command<NavigationOption<T>>(execute), delay)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The action to perform.</param>
        /// <param name="canExecute">A callback to determine whether the action can be performed.</param>
        /// <param name="delay">The delay until another command can be executed (default is 300ms).</param>
        public NavigateCommand(Action<NavigationOption<T>> execute, Func<NavigationOption<T>, bool> canExecute, int delay = 300) : base(new Xamarin.Forms.Command<NavigationOption<T>>(execute, canExecute), delay)
        {
        }
    }
}
