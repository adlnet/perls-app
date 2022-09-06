using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using TinCan;
using TinCan.Documents;
using Xamarin.Forms;

namespace PERLS.Data.ExperienceAPI
{
    /// <summary>
    /// Provides access to state documents.
    /// </summary>
    public class ExperienceStateProvider : IStateService
    {
        readonly ILRSService lrsService = DependencyService.Get<ILRSService>();
        readonly Agent agent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExperienceStateProvider"/> class.
        /// </summary>
        public ExperienceStateProvider()
        {
            agent = DependencyService.Get<IAppContextService>().LearnerAgent;
        }

        /// <inheritdoc />
        public async void SavePosition(IItem item, double position)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var content = new PositionState() { Position = position };
            var serializedContent = JsonConvert.SerializeObject(content);

            var doc = new StateDocument
            {
                activity = ActivityBuilder.FromResource(item),
                agent = agent,
                id = item.Id.ToString(),
                content = System.Text.Encoding.UTF8.GetBytes(serializedContent),
                contentType = "application/json",
            };

            var response = await lrsService.SaveState(doc).ConfigureAwait(false);

            if (!response.success)
            {
                if (response.httpException != null)
                {
                    throw response.httpException;
                }

                throw new SendStatementException(response.errMsg ?? Strings.StatementSendError);
            }

            return;
        }

        /// <inheritdoc />
        public async Task<double> RetrievePosition(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var response = await lrsService.RetrieveState(item.Id.ToString(), ActivityBuilder.FromResource(item), agent).ConfigureAwait(false);

            if (!response.success)
            {
                if (response.httpException != null)
                {
                    throw response.httpException;
                }

                throw new SendStatementException(response.errMsg ?? Strings.StatementSendError);
            }

            var retreivedDoc = response.content;

            if (retreivedDoc.content != null)
            {
                var stored = System.Text.Encoding.UTF8.GetString(retreivedDoc.content);
                var positionState = JsonConvert.DeserializeObject<PositionState>(stored);
                return positionState.Position;
            }

            return 0.0;
        }

        /// <inheritdoc />
        public async void DeletePosition(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var doc = new StateDocument
            {
                activity = ActivityBuilder.FromResource(item),
                agent = agent,
                id = item.Id.ToString(),
                contentType = "application/json",
            };

            var response = await lrsService.DeleteState(doc).ConfigureAwait(false);

            if (!response.success)
            {
                if (response.httpException != null)
                {
                    throw response.httpException;
                }

                throw new SendStatementException(response.errMsg ?? Strings.StatementSendError);
            }

            return;
        }
    }
}
