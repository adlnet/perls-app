using System;
using System.Collections.Generic;
using System.Linq;
using Float.Core.UX;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using NavigationEventArgs = Float.Core.UX.NavigationEventArgs;

namespace PERLS
{
    /// <summary>
    /// A base navigation context for a standard INavigation handler.
    /// </summary>
    /// <remarks>This is a candidate for moving into Float.Core.</remarks>
    public abstract class BaseNavigationContext : INavigationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNavigationContext"/> class.
        /// </summary>
        /// <param name="navigationHandler">The platform-specific navigation handler.</param>
        protected BaseNavigationContext(INavigation navigationHandler)
        {
            NavigationHandler = navigationHandler ?? throw new ArgumentNullException(nameof(navigationHandler));
        }

        /// <inheritdoc />
        public event EventHandler<NavigationEventArgs> Navigated;

        /// <inheritdoc />
        public bool IsAtRootPage => NavigationHandler.NavigationStack.Count == 1 && NavigationHandler.ModalStack.Count == 0;

        /// <summary>
        /// Gets the platform-specific navigation handler.
        /// </summary>
        /// <value>The navigation handler.</value>
        protected INavigation NavigationHandler { get; }

        /// <inheritdoc />
        public void PushPage(Page page, bool animated = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NavigationHandler.PushAsync(page, animated);

                if (PopupNavigation.Instance.PopupStack.Any() && !page.IsBusy)
                {
                    PopupNavigation.Instance.PopAllAsync();
                }
            });
        }

        /// <inheritdoc />
        public void PopPage(bool animated = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NavigationHandler.PopAsync(animated);
            });
        }

        /// <inheritdoc />
        public void PresentPage(Page page, bool animated = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NavigationHandler.PushModalAsync(page, animated);
            });
        }

        /// <inheritdoc />
        public void DismissPage(bool animated = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NavigationHandler.PopModalAsync(animated);
            });
        }

        /// <inheritdoc />
        public virtual void Reset(bool animated = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NavigationHandler.PopToRootAsync(animated);
            });
        }

        /// <inheritdoc />
        public void ShowDetailPage(Page page, bool animated = true)
        {
            PushPage(page, animated);
        }

        /// <inheritdoc />
        public void ShowOverviewPage(Page page, bool animated = true)
        {
            PushPage(page, animated);
        }

        /// <summary>
        /// Dispatches the <see cref="Navigated"/> event.
        /// </summary>
        /// <param name="args">The navigation event.</param>
        protected void SendNavigatedEvent(NavigationEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Navigated?.Invoke(this, args);

            // Pages sometimes take a really long time to get cleaned up.
            // This is inteded to help view models clean up a bit quicker
            // by clearing the binding context of a page when it is popped
            // from the navigation stack.
            if (args.Type == NavigationEventArgs.NavigationType.Popped)
            {
                if (args.Page is Page page)
                {
                    page.BindingContext = null;

                    if (page.ToolbarItems is IEnumerable<ToolbarItem> items)
                    {
                        foreach (var item in items)
                        {
                            item.BindingContext = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Captures the state of the navigation stack.
        /// </summary>
        /// <returns>The navigation stack state.</returns>
        protected NavigationStackState CaptureNavigationStackState()
        {
            return CaptureStackState(NavigationHandler.NavigationStack);
        }

        /// <summary>
        /// Captures the state of the modal stack.
        /// </summary>
        /// <returns>The modal stack state.</returns>
        protected NavigationStackState CaptureModalStackState()
        {
            return CaptureStackState(NavigationHandler.ModalStack);
        }

        /// <summary>
        /// Captures the state of a stack.
        /// </summary>
        /// <param name="stack">The stack to capture the state of.</param>
        /// <returns>The navigation state.</returns>
        static NavigationStackState CaptureStackState(IReadOnlyList<Page> stack)
        {
            return new NavigationStackState
            {
                StackCount = stack.Count,
                StackTopPage = stack.Count > 0 ? stack[stack.Count - 1] : null,
            };
        }

        /// <summary>
        /// Represents the state of a navigation stack so that navigation events can be determined.
        /// </summary>
        protected class NavigationStackState
        {
            /// <summary>
            /// Gets the number of pages on the stack.
            /// </summary>
            /// <value>The number of pages on the stack.</value>
            public int StackCount { get; internal set; }

            /// <summary>
            /// Gets the top-most page.
            /// </summary>
            /// <value>The page on top of the stack.</value>
            public Page StackTopPage { get; internal set; }

            /// <summary>
            /// Gets the navigation event for the difference between this state and an old state.
            /// </summary>
            /// <param name="oldState">The old navigation state.</param>
            /// <returns>The navigation event.</returns>
            public NavigationEventArgs GetNavigationEvent(NavigationStackState oldState)
            {
                if (oldState == null)
                {
                    throw new ArgumentNullException(nameof(oldState));
                }

                int delta = StackCount - oldState.StackCount;

                if (delta > 0)
                {
                    return new NavigationEventArgs(NavigationEventArgs.NavigationType.Pushed, StackTopPage);
                }

                if (delta == -1)
                {
                    return new NavigationEventArgs(NavigationEventArgs.NavigationType.Popped, oldState.StackTopPage);
                }

                if (delta < -1)
                {
                    return new NavigationEventArgs(NavigationEventArgs.NavigationType.Reset, StackTopPage);
                }

                return null;
            }
        }
    }
}
