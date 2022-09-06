using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Net;
using Float.TinCan.QueuedLRS;
using PERLS.Data;
using PERLS.Data.Definition.Services;
using PERLS.Data.ExperienceAPI;
using PERLS.Data.Extensions;
using TinCan;
using TinCan.Documents;
using TinCan.LRSResponses;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// The LRS for the application.
    /// </summary>
    public class LRSProxy : ILRS
    {
        readonly RemoteLRS targetLRS;

        /// <summary>
        /// Initializes a new instance of the <see cref="LRSProxy"/> class.
        /// </summary>
        public LRSProxy()
        {
            var appContext = DependencyService.Get<IAppContextService>();
            var endpoint = new Uri(appContext.Server, "lrs");
            targetLRS = new RemoteLRS(endpoint, TCAPIVersion.latest(), string.Empty, string.Empty);
        }

        /// <summary>
        /// Checks if the current LRS endpoint is equal to the provided uri.
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <returns>Returns <cref>true</cref> if the uri's match.</returns>
        public bool CheckEndpoint(Uri uri)
        {
            if (uri == null)
            {
                return false;
            }

            if (targetLRS.endpoint.Equals(uri))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the current LRS endpoint with the provided uri.
        /// </summary>
        /// <param name="uri">The URI to replace the current URI.</param>
        public void UpdateEndpoint(Uri uri)
        {
            if (uri == null)
            {
                return;
            }

            if (targetLRS.endpoint == uri)
            {
                return;
            }

            targetLRS.endpoint = uri;
        }

        /// <inheritdoc />
        public async Task<AboutLRSResponse> About()
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.About().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<LRSResponse> ClearState(Activity activity, Agent agent, Guid? registration = null)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.ClearState(activity, agent, registration).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<LRSResponse> DeleteActivityProfile(ActivityProfileDocument profile)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.DeleteActivityProfile(profile).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<LRSResponse> DeleteAgentProfile(AgentProfileDocument profile)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.DeleteAgentProfile(profile).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<LRSResponse> DeleteState(StateDocument state)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.DeleteState(state).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<LRSResponse> ForceSaveAgentProfile(AgentProfileDocument profile)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.ForceSaveAgentProfile(profile).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<StatementsResultLRSResponse> MoreStatements(StatementsResult result)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.MoreStatements(result).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<StatementsResultLRSResponse> QueryStatements(StatementsQuery query)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.QueryStatements(query).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ActivityProfileLRSResponse> RetrieveActivityProfile(string id, Activity activity)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.RetrieveActivityProfile(id, activity).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ProfileKeysLRSResponse> RetrieveActivityProfileIds(Activity activity)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.RetrieveActivityProfileIds(activity).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<AgentProfileLRSResponse> RetrieveAgentProfile(string id, Agent agent)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.RetrieveAgentProfile(id, agent).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ProfileKeysLRSResponse> RetrieveAgentProfileIds(Agent agent)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.RetrieveAgentProfileIds(agent).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<StateLRSResponse> RetrieveState(string id, Activity activity, Agent agent, Guid? registration = null)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.RetrieveState(id, activity, agent, registration).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ProfileKeysLRSResponse> RetrieveStateIds(Activity activity, Agent agent, Guid? registration = null)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.RetrieveStateIds(activity, agent, registration).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<StatementLRSResponse> RetrieveStatement(Guid id)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.RetrieveStatement(id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<StatementLRSResponse> RetrieveVoidedStatement(Guid id)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.RetrieveVoidedStatement(id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<LRSResponse> SaveActivityProfile(ActivityProfileDocument profile)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.SaveActivityProfile(profile).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<LRSResponse> SaveAgentProfile(AgentProfileDocument profile)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.SaveAgentProfile(profile).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<LRSResponse> SaveState(StateDocument state)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.SaveState(state).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<StatementLRSResponse> SaveStatement(Statement statement)
        {
            // There seems to be an issue in our Drupal LRS proxy where saving a single statement does not work correctly.
            // No matter--we'll just always save statements in batches.
            var result = await SaveStatements(new List<Statement> { statement }).ConfigureAwait(false);
            return new StatementLRSResponse
            {
                success = result.success,
                content = statement,
                httpException = result.httpException,
            };
        }

        /// <inheritdoc />
        public async Task<StatementsResultLRSResponse> SaveStatements(List<Statement> statements)
        {
            if (statements == null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            foreach (var statement in statements)
            {
                HandleStatement(statement);
            }

            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.SaveStatements(statements).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<StatementLRSResponse> VoidStatement(Guid id, Agent agent)
        {
            await UpdateAuthorization().ConfigureAwait(false);
            return await targetLRS.VoidStatement(id, agent).ConfigureAwait(false);
        }

        void HandleStatement(Statement statement)
        {
            // This is to conform with XAPI-00085: https://github.com/adlnet/xapi-lrs-conformance-requirements/blob/master/20_statements/24_statement_properties.md
            // A Statement cannot contain a "platform" property in its "context" property and have the value of the "object" property's "objectType" be anything but "Activity".
            if (statement.target.ObjectType != "Activity")
            {
                return;
            }

            if (statement.context == null)
            {
                statement.context = new Context();
            }

            var context = DependencyService.Get<IAppContextService>();
            statement.context.platform = $"{context.Name} ({context.Version})";
        }

        async Task UpdateAuthorization()
        {
            var network = DependencyService.Get<INetworkConnectionService>();

            if (network.AuthStrategy is not OAuth2StrategyBase authStrategy)
            {
                return;
            }

            var token = await authStrategy.GetAccessToken().ConfigureAwait(false);
            targetLRS.auth = $"Bearer {token}";
        }
    }
}
