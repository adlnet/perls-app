using System;
using Float.Core.Notifications;
using Float.Core.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// The base class to use for popup pages.
    /// </summary>
    /// <remarks>Similar to BasePage.</remarks>
    public abstract class BasePopupPage : PopupPage
    {
        /// <summary>
        /// Binding for the Error property.
        /// </summary>
        public static readonly BindableProperty ErrorProperty = BindableProperty.Create(nameof(Error), typeof(Exception), typeof(ContentPage), null);

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePopupPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to use as the binding context.</param>
        protected BasePopupPage(IPageViewModel viewModel)
        {
            BindingContext = viewModel;
        }

        /// <summary>
        /// Gets or sets the current error on the page.
        /// </summary>
        /// <value>The exception that has occurred which the user should be aware of.</value>
        protected Exception Error
        {
            get => GetValue(ErrorProperty) as Exception;
            set => SetValue(ErrorProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            FlowDirection = Device.FlowDirection;
        }

        /// <inheritdoc />
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is IPageViewModel viewModel)
            {
                this.SetBinding(TitleProperty, nameof(viewModel.Title));
                this.SetBinding(IsBusyProperty, nameof(viewModel.IsLoading));
                this.SetBinding(ErrorProperty, nameof(viewModel.Error), BindingMode.TwoWay);
            }
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Error):
                    OnError(Error);
                    break;
            }
        }

        /// <summary>
        /// Invoked when an error occurs--probably an error performing a user request.
        /// </summary>
        /// <param name="exception">The error that occurred.</param>
        protected virtual void OnError(Exception exception)
        {
            if (exception != null)
            {
                DependencyService.Get<INotificationHandler>().NotifyException(exception, Data.Strings.DefaultErrorTitle);
            }
        }
    }
}
