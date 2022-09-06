using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Float.Core.Analytics;
using Float.Core.Net;
using Float.Core.Persistence;
using PERLS.Data;
using PERLS.Data.Definition.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// The implementation for the Network Connection Service dependency service.
    /// </summary>
    public class NetworkConnectionService : INetworkConnectionService
    {
        AsyncLazy<bool> reachable;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConnectionService"/> class.
        /// </summary>
        public NetworkConnectionService()
        {
            reachable = new AsyncLazy<bool>(UpdateReachabilityState);
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
            RefreshAuthStrategy();
        }

        /// <inheritdoc />
        public IAuthStrategy AuthStrategy { get; set; }

        /// <inheritdoc />
        public Uri BaseUri => AppConfig.Server;

        Uri OAuthRedirectUri => new Uri(Constants.OAuthRedirectUri);

        string LoginEndpoint => "oauth/token";

        string LocalLoginEndpoint => "user/login";

        string AuthorizeEndpoint => "oauth/authorize";

        string RevokeEndpoint => "oauth/revoke";

        /// <inheritdoc />
        public async Task<bool> IsReachable() => await reachable.Value;

        /// <inheritdoc />
        public Uri GetAuthorizationUrl(bool requestLocalAuthentication = false)
        {
            if (AuthStrategy is not OAuth2StrategyAuthCode authCodeStrategy)
            {
                return null;
            }

            var authUrl = authCodeStrategy.GetAuthorizationCodeURL();

            if (requestLocalAuthentication)
            {
                var query = HttpUtility.ParseQueryString(string.Empty);

                // Until #SL-4266 is resolved, the order of these parameters is important.
                query["destination"] = authUrl.PathAndQuery;
                query["local"] = "true";

                var localAuth = new UriBuilder(BaseUri)
                {
                    Path = LocalLoginEndpoint,
                    Query = query.ToString(),
                };
                return localAuth.Uri;
            }

            return authUrl;
        }

        /// <summary>
        /// Refreshes and updates the AuthStrategy.
        /// </summary>
        public void RefreshAuthStrategy()
        {
            var secureService = DependencyService.Get<ISecureStore>();

            AuthStrategy = new OAuth2StrategyAuthCode(AppConfig.Server, Constants.ClientId, Constants.ClientSecret, secureService, OAuthRedirectUri, AuthorizeEndpoint, LoginEndpoint, RevokeEndpoint);
        }

        void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs args)
        {
            reachable = new AsyncLazy<bool>(UpdateReachabilityState);
        }

        bool UpdateReachabilityState()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                return false;
            }

            // we use .Result in a few places to make this synchronous; not ideal, but this should be a quick call
            try
            {
                var config = new ClientHandlerConfiguration
                {
                    AllowCookies = false,
                };
                var handler = DependencyService.Get<INativeHttpClientHandler>().CreateHandler(config);
                using (var client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 10) })
                using (var request = new HttpRequestMessage(HttpMethod.Head, new Uri(BaseUri, "ping")))
                using (var response = client.SendAsync(request).Result)
                {
                    return (int)response.StatusCode > 0;
                }
            }
            catch (Exception e) when (e is AggregateException || e is TaskCanceledException || e is System.Net.Http.HttpRequestException)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
                reachable = new AsyncLazy<bool>(UpdateReachabilityState);
            }

            return false;
        }
    }
}
