using System;
using System.Linq;
using Float.Core.Analytics;
using Float.Core.Notifications;
using PERLS.Components;
using PERLS.Data.ViewModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// Base page implementation.
    /// </summary>
    public abstract class BasePage : Float.Core.UI.BaseContentPage
    {
        static DateTime timeOfLastError = DateTime.MinValue;

        readonly LoadingIndicatorView loadingIndicatorView = new LoadingIndicatorView();

        /// <summary>
        /// A value indicating whether a popup is currently floating over this page.
        /// </summary>
        bool isPopupShowing;

        bool isPageVisible;

        LogoNavBarView navBarView;

        double loadedWidth;

        /// <summary>
        /// Gets a value indicating whether landscape orientation should be allowed.
        /// </summary>
        /// <value><c>true</c> if landscape orientation should be allowed, <c>false</c> otherwise.</value>
        public virtual bool AllowLandscape => Device.Idiom == TargetIdiom.Tablet;

        /// <summary>
        /// Gets a value indicating whether the navigation and tab bar should be hidden while in landscape.
        /// </summary>
        /// <value><c>true</c> if the navigation bar and tab bar should be hidden in landscape.</value>
        public virtual bool HidesBarsInLandscape { get; }

        /// <summary>
        /// Gets a value indicating whether the header logo should be visible.
        /// </summary>
        /// <value><c>true</c> if the header logo should be visible, <c>false</c> otherwise.</value>
        public virtual bool ShowLogo { get; }

        /// <summary>
        /// Gets a value indicating whether the page uses a custom navigation bar.
        /// </summary>
        /// <value><c>true</c> if the nav bar should be used, <c>false</c> otherwise.</value>
        /// <remarks>
        /// The custom navigation bar will display the title in large text
        /// and display the current user's avatar.
        /// </remarks>
        protected virtual bool UsesCustomNavigationBar { get; }

        /// <summary>
        /// Gets the toolbar item representing an optional action.
        /// </summary>
        /// <value>The toolbar item.</value>
        protected ToolbarItem AlternateActionToolbarItem { get; private set; }

        /// <summary>
        /// Gets the current shell containing the page.
        /// </summary>
        /// <value>The current shell.</value>
        protected Shell CurrentShell
        {
            get
            {
                Element shell = this;

                do
                {
                    shell = shell.Parent;
                }
                while (!(shell is Shell) && shell is BaseShellItem);

                return shell as Shell;
            }
        }

        /// <summary>
        /// Called when the page has finished appearing.
        /// </summary>
        public virtual void OnAppeared()
        {
        }

        /// <summary>
        /// Called when the page has finished disappearing.
        /// </summary>
        public virtual void OnDisappeared()
        {
        }

        /// <summary>
        /// Called when the page has finished rotating.
        /// </summary>
        public virtual void OnFinishedRotation()
        {
        }

        /// <summary>
        /// Called when the page is being unloaded.
        /// </summary>
        public virtual void OnUnload()
        {
        }

        /// <inheritdoc />
        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (CurrentShell is CorpusShell shell)
            {
                // TODO: This should not be necessary...the shell's toolbar items should be added automatically.
                // Only apply the toolbar items to the top-level items in the shell.
                if (Parent is ShellContent)
                {
                    foreach (var item in shell.ToolbarItems)
                    {
                        ToolbarItems.Add(item);
                    }

                    // Provide more padding on iOS devices between tabs and content.
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        if (Content.Margin is Thickness margin)
                        {
                            margin.Top += (double)Application.Current.Resources["Spacing"];
                            Content.Margin = margin;
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is IActionableProvider provider && provider.Action != null)
            {
                if (AlternateActionToolbarItem == null)
                {
                    AlternateActionToolbarItem = new ToolbarItem();
                    ToolbarItems.Add(AlternateActionToolbarItem);
                }

                AlternateActionToolbarItem.BindingContext = provider.Action;
                AlternateActionToolbarItem.SetBinding(MenuItem.TextProperty, nameof(IActionableViewModel.ActionLabel));
                AlternateActionToolbarItem.SetBinding(MenuItem.CommandProperty, nameof(IActionableViewModel.ActionCommand));
                AlternateActionToolbarItem.SetBinding(MenuItem.CommandParameterProperty, nameof(IActionableViewModel<object>.ActionParameter));
            }
            else if (AlternateActionToolbarItem != null)
            {
                ToolbarItems.Remove(AlternateActionToolbarItem);
                AlternateActionToolbarItem = null;
            }
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            isPageVisible = (CurrentShell as CorpusShell)?.IsCurrent != false;
            StartObservingPopups();

            if (UsesCustomNavigationBar)
            {
                navBarView = new LogoNavBarView(ShowLogo, Title);
                Shell.SetTitleView(this, navBarView);
            }

            if (IsBusy)
            {
                OnStartLoading();
            }
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            isPageVisible = false;

            if (!isPopupShowing)
            {
                StopObservingPopups();
            }

            if (navBarView is LogoNavBarView view)
            {
                view.OnDisappearing();
            }
        }

        /// <inheritdoc />
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (CurrentShell != null && HidesBarsInLandscape)
            {
                var isLandscape = DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape;

                // We're likely already in the middle of a layout pass right now,
                // so we'll request a change of visibility on the nav/tab bar
                // on the _next_ run loop.
                // In practice, this avoids an issue on Android where without
                // the delay, there would be an empty space where the nav/tab bar
                // used to be.
                Device.BeginInvokeOnMainThread(() =>
                {
                    Shell.SetNavBarIsVisible(this, !isLandscape);
                    Shell.SetTabBarIsVisible(this, !isLandscape);
                });
            }
        }

        /// <inheritdoc/>
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);

            if (loadedWidth == 0)
            {
                loadedWidth = width;
                return;
            }

            if (loadedWidth != width)
            {
                loadedWidth = width;

                Device.BeginInvokeOnMainThread(() =>
                {
                    var titleView = Shell.GetTitleView(this);

                    if (titleView != null)
                    {
                        Shell.SetTitleView(this, null);
                        Shell.SetTitleView(this, titleView);
                    }
                });
            }
        }

        /// <inheritdoc />
        protected override void OnStartLoading()
        {
            base.OnStartLoading();

            if (!isPageVisible || PopupNavigation.Instance.PopupStack.Contains(loadingIndicatorView))
            {
                return;
            }

            // Catch null reference exception that happens when you start ios in landscape and discard it.  Not much else I saw you can do because it happens through the way its suppose to be called in onappearing.  Null reference seems to have to do with invoke on main thread called inside this.
            try
            {
                PopupNavigation.Instance.PushAsync(loadingIndicatorView);
            }
            catch (NullReferenceException e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }
        }

        /// <inheritdoc />
        protected override void OnStopLoading()
        {
            base.OnStopLoading();

            if (!PopupNavigation.Instance.PopupStack.Contains(loadingIndicatorView))
            {
                return;
            }

            try
            {
                PopupNavigation.Instance.RemovePageAsync(loadingIndicatorView);
            }
            catch (Exception e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }
        }

        /// <inheritdoc />
        protected override void OnError(Exception exception)
        {
            if (exception != null)
            {
                var httpException = exception.InnerException as Float.Core.Net.HttpRequestException;
                if ((httpException == null || httpException.Code != 401)
                    && !(exception.InnerException is System.Net.Http.HttpRequestException)
                    && timeOfLastError < DateTime.Now.AddSeconds(-2)
                    && isPageVisible)
                {
                    DependencyService.Get<INotificationHandler>().NotifyException(exception, Data.Strings.DefaultErrorTitle);
                }

                timeOfLastError = DateTime.Now;
            }
        }

        void StartObservingPopups()
        {
            // Safeguard to avoid adding duplicate event handlers.
            StopObservingPopups();

            PopupNavigation.Instance.Pushing += OnPopupAppearing;
            PopupNavigation.Instance.Popping += OnPopupDisappearing;
        }

        void StopObservingPopups()
        {
            PopupNavigation.Instance.Pushing -= OnPopupAppearing;
            PopupNavigation.Instance.Popping -= OnPopupDisappearing;
        }

        void OnPopupDisappearing(object sender, Rg.Plugins.Popup.Events.PopupNavigationEventArgs e)
        {
            if (!isPopupShowing || !(e.Page is SearchPopUpPage || e.Page is BadgeDetailPopupPage))
            {
                return;
            }

            isPopupShowing = false;
            SendAppearing();
        }

        void OnPopupAppearing(object sender, Rg.Plugins.Popup.Events.PopupNavigationEventArgs e)
        {
            if (isPopupShowing || !(e.Page is SearchPopUpPage || e.Page is BadgeDetailPopupPage))
            {
                return;
            }

            isPopupShowing = true;
            SendDisappearing();
        }
    }
}
