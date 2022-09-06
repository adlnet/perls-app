using Float.Core.Net;
using PERLS.Data;
using PERLS.Data.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace PERLS.Pages
{
    /// <summary>
    /// The Authenticate Page, used for showing a webview with an OAuth login page.
    /// </summary>
    public partial class AuthenticatePage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatePage"/> class.
        /// </summary>
        /// <param name="viewModel">The login view model.</param>
        public AuthenticatePage(LoginViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            var networkInfo = DependencyService.Get<Data.Definition.Services.INetworkConnectionService>();
            if (networkInfo.AuthStrategy is OAuth2StrategyAuthCode oauthStrategy)
            {
                webView.Navigating += oauthStrategy.CheckForAuthorizationCode;
            }

            NavigationPage.SetHasBackButton(this, false);
            var cancel = new ToolbarItem(Strings.Cancel, null, () => this.Navigation.PopAsync());
            ToolbarItems.Add(cancel);
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
        }
    }
}
