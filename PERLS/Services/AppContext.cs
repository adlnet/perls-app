using System;
using System.Threading.Tasks;
using Float.Core.Analytics;
using Float.Core.Net;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using TinCan;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// App context.
    /// </summary>
    public class AppContext : IAppContextService
    {
        /// <inheritdoc />
        public string Name => Xamarin.Essentials.AppInfo.Name;

        /// <inheritdoc />
        public string Identifier => Xamarin.Essentials.AppInfo.PackageName;

        /// <inheritdoc />
        public string BuildNumber => Xamarin.Essentials.AppInfo.BuildString;

        /// <inheritdoc />
        public string Version => Constants.FullSemVersion;

        /// <inheritdoc />
        public string PackageIdentifier => Xamarin.Essentials.AppInfo.PackageName;

        /// <inheritdoc />
        public Uri Server => API.BaseUri;

        /// <inheritdoc />
        /// <remarks>Consider adding an event to notify observers when the current learner changes.</remarks>
        /// <remarks>TODO: This should save the value and restore across application launches.</remarks>
        public ILearner CurrentLearner => LearnerProxy;

        /// <inheritdoc />
        public Agent LearnerAgent
        {
            get
            {
                return new Agent
                {
                    account = new AgentAccount
                    {
                        name = CurrentLearner.Id.ToString(),
                        homePage = Server,
                    },
                };
            }
        }

        /// <inheritdoc />
        public Agent SystemAgent
        {
            get
            {
                return new Agent
                {
                    name = Name,
                    account = new AgentAccount
                    {
                        name = Name,
                        homePage = Server,
                    },
                };
            }
        }

        /// <summary>
        /// Gets a reference to the current network service.
        /// </summary>
        /// <value>The network service.</value>
        public INetworkConnectionService API => DependencyService.Get<INetworkConnectionService>();

        /// <inheritdoc />
        public bool IsLearnerAuthenticated => API.AuthStrategy.IsAuthenticated;

        /// <inheritdoc />
        public string UserAgentSuffix => FormattableString.Invariant($"{Constants.UserAgent}/{Version} ({Xamarin.Essentials.AppInfo.Name})");

        private LearnerProxy LearnerProxy { get; } = new LearnerProxy();

        /// <inheritdoc />
        public void Logout()
        {
            LearnerProxy.Source = null;
            API.AuthStrategy.Logout();
        }

        /// <inheritdoc />
        public async Task<ILearner> RefreshLearnerProfile()
        {
            try
            {
                var learner = await DependencyService.Get<ILearnerProvider>().GetCurrentLearner().ConfigureAwait(false);
                LearnerProxy.Source = learner;
            }
            catch (HttpRequestException e) when (e.Code >= 400 && e.Code < 500)
            {
                LearnerProxy.Source = null;
                throw;
            }
            catch (Exception e) when (LearnerProxy.Source != null)
            {
                // If an exception occurs while loading the learner profile and it was _not_
                // a 4xx error (i.e. a 401 or 403), then something is wrong either with the server
                // or the Internet connection. In either case, we can ignore the error.
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }

            return CurrentLearner;
        }
    }
}
