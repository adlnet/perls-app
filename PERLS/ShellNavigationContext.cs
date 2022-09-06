using System;
using System.Linq;
using Float.Core.Extensions;
using Xamarin.Forms;
using NavigationEventArgs = Float.Core.UX.NavigationEventArgs;

namespace PERLS
{
    /// <summary>
    /// Navigation context for a Xamarin Forms Shell.
    /// </summary>
    /// <remarks>
    /// The Navigating and Navigated events for Shell are very buggy and are not reliable at all.
    /// Currently the Navigated events coming out of here are merely best attempts at notifying
    /// handlers that the navigation state has changed--but it's unreliable.
    /// </remarks>
    public class ShellNavigationContext : BaseNavigationContext, IDisposable
    {
        Shell shell;
        Page previousTopVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellNavigationContext"/> class.
        /// </summary>
        /// <param name="shell">The shell.</param>
        public ShellNavigationContext(Shell shell) : base(GetNavigation(shell))
        {
            // NOTE: Xamarin Forms shell navigation events are very buggy.
            this.shell = shell;
            shell.Navigating += HandleShellNavigating;
            shell.Navigated += HandleShellNavigated;
        }

        /// <inheritdoc />
        public override void Reset(bool animated = true)
        {
            base.Reset(animated);

            Device.BeginInvokeOnMainThread(() =>
            {
                shell?.Items?
                    .SelectMany(item => item?.Items)
                    .SelectMany(section => section?.Items)
                    .Select(content => content?.Navigation)
                    .ForEach(navigation => navigation?.PopToRootAsync(animated));
            });

            SendNavigatedEvent(new NavigationEventArgs(NavigationEventArgs.NavigationType.Reset, null));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            shell.Navigating -= HandleShellNavigating;
            shell.Navigated -= HandleShellNavigated;
            shell = null;
        }

        static INavigation GetNavigation(Shell shell)
        {
            if (shell == null)
            {
                throw new ArgumentNullException(nameof(shell));
            }

            return shell.Navigation;
        }

        void HandleShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            previousTopVisible = GetVisiblePage(sender as Shell);
        }

        void HandleShellNavigated(object sender, ShellNavigatedEventArgs e)
        {
            NavigationEventArgs navigationEventArgs = null;

            switch (e.Source)
            {
                case ShellNavigationSource.Push:
                    if (GetVisiblePage(sender as Shell) is Page pushedPage)
                    {
                        navigationEventArgs = new NavigationEventArgs(NavigationEventArgs.NavigationType.Pushed, pushedPage);
                    }

                    break;
                case ShellNavigationSource.Pop:
                    navigationEventArgs = new NavigationEventArgs(NavigationEventArgs.NavigationType.Popped, previousTopVisible);
                    break;
                case ShellNavigationSource.PopToRoot:
                    navigationEventArgs = new NavigationEventArgs(NavigationEventArgs.NavigationType.Reset, GetVisiblePage(sender as Shell));
                    break;
            }

            if (navigationEventArgs is NavigationEventArgs args)
            {
                SendNavigatedEvent(args);
            }

            previousTopVisible = null;
        }

        Page GetVisiblePage(Shell shell)
        {
            if (shell == null)
            {
                return null;
            }

            var nav = shell.Navigation;
            var stack = nav.ModalStack.Any() ? nav.ModalStack : nav.NavigationStack;

            return stack?.LastOrDefault();
        }
    }
}
