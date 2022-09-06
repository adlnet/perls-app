using System;
using System.Windows.Input;
using Float.Core.Events;
using Float.Core.Extensions;
using Float.Core.Net;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The base view model for the onboarding Create Account page.
    /// </summary>
    public class LoginViewModel : BasePageViewModel
    {
        readonly INetworkConnectionService networkInfo = DependencyService.Get<INetworkConnectionService>();
        bool didAuthorize;
        bool inSingleSignOn;
        bool didReceiveWebNavigated;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="loginSelectedCommand">The command for login being selected.</param>
        /// <param name="userDidLoginCommand">The command for login being completed.</param>
        /// <param name="userFailedToLoginCommand">The command for login failing.</param>
        /// <param name="pageFailedToLoad">The command for the page failing to load.</param>
        /// <param name="enableLocalAuthentication">Whether the app should use local authentication.</param>
        public LoginViewModel(ICommand loginSelectedCommand, ICommand userDidLoginCommand, ICommand userFailedToLoginCommand, ICommand pageFailedToLoad, bool enableLocalAuthentication)
        {
            SelectLoginCommand = loginSelectedCommand ?? throw new ArgumentNullException(nameof(loginSelectedCommand));
            UserDidLoginCommand = userDidLoginCommand ?? throw new ArgumentNullException(nameof(userDidLoginCommand));
            UserFailedToLoginCommand = userFailedToLoginCommand ?? throw new ArgumentNullException(nameof(userFailedToLoginCommand));
            EnableLocalAuthentication = enableLocalAuthentication;

            if (networkInfo.AuthStrategy is OAuth2StrategyAuthCode authCodeStrategy)
            {
                authCodeStrategy.OnAuthCodeReceived += OnAuthorizationCodeReceived;
            }

            LoadingEvent = new Command((obj) =>
            {
                if (obj is WebNavigatingEventArgs args1)
                {
                    // if the user is not navigating to the login host or using our auth endpoint,
                    // they're being redirected elsewhere and we don't want to deal with it
                    // calling the login failure command with a URI should open that URI in the device browser
                    var targetUri = new Uri(args1.Url);

                    if (targetUri.LocalPath == "/saml_login")
                    {
                        inSingleSignOn = true;
                    }
                    else
                    {
                        if (!(targetUri.Host == LoginUri.Host || targetUri.LocalPath == "/auth") && !inSingleSignOn && didReceiveWebNavigated)
                        {
                            userFailedToLoginCommand.Execute(targetUri);
                            args1.Cancel = true;
                            return;
                        }
                    }
                }

                if (inSingleSignOn && Device.RuntimePlatform == Device.Android)
                {
                    IsLoading = false;
                }
                else
                {
                    IsLoading = obj is WebNavigatingEventArgs;
                }

                didReceiveWebNavigated = obj is WebNavigatedEventArgs;

                if (obj is WebNavigatedEventArgs args && args.Result != WebNavigationResult.Success && !didAuthorize)
                {
                    var exception = new HttpConnectionException($"Unable to load authentication page: {LoginUri}");
                    Error = exception;
                    pageFailedToLoad.Execute(exception);
                }
            });
        }

        /// <summary>
        /// Gets or sets a value indicating whether the app should use local authentication.
        /// </summary>
        /// <value>Whether the app should use local authentication.</value>
        public bool EnableLocalAuthentication { get; set; }

        /// <summary>
        /// Gets the command to invoke when login is selected.
        /// </summary>
        /// <value>The login command.</value>
        public ICommand SelectLoginCommand { get; }

        /// <summary>
        /// Gets the command to invoke when login is completed.
        /// </summary>
        /// <value>The logged in command.</value>
        public ICommand UserDidLoginCommand { get; }

        /// <summary>
        /// Gets the command to invoke when the user has failed to login.
        /// </summary>
        /// <value>The login failed command.</value>
        public ICommand UserFailedToLoginCommand { get; }

        /// <summary>
        /// Gets the loading event for the WebView.
        /// </summary>
        /// <value>The loading event command.</value>
        public ICommand LoadingEvent { get; }

        /// <summary>
        /// Gets the Uri for logging in.
        /// </summary>
        /// <value>The login URI.</value>
        public Uri LoginUri => networkInfo.GetAuthorizationUrl(EnableLocalAuthentication);

        /// <summary>
        /// Gets the title text for the login button.
        /// </summary>
        /// <value>The login button title.</value>
        public string LoginButtonTitle => Strings.SignInLabel;

        /// <inheritdoc/>
        public override void Refresh()
        {
            OnPropertyChanged(nameof(LoginUri));
        }

        /// <summary>
        /// This callback function is called when the authorization code is received by the AuthStrategy.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">Contains the authorization code needed for login.</param>
        public void OnAuthorizationCodeReceived(object sender, TypedEventArgs<string> args)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            didAuthorize = true;
            var code = args.Data;
            (sender as OAuth2StrategyAuthCode).OnAuthCodeReceived -= OnAuthorizationCodeReceived;
            _ = (sender as OAuth2StrategyAuthCode).Login(code, string.Empty).OnSuccess(loginTask =>
              {
                  IsLoading = false;
                  UserDidLoginCommand.Execute(null);
              }).OnFailure((task) =>
              {
                  UserFailedToLoginCommand.Execute(null);
              });
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(EnableLocalAuthentication):
                    OnPropertyChanged(nameof(LoginUri));
                    break;
            }
        }
    }
}
