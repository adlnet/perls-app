using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Float.TinCan.QueuedLRS;
using Float.TinCan.QueuedLRS.Stores;
using Float.TinCan.QueuedLRS.Triggers;
using PERLS.Data;
using PERLS.Data.Definition.Services;
using PERLS.Data.ExperienceAPI;
using PERLS.Data.ExperienceAPI.Profiles.Perls;
using PERLS.Data.Extensions;
using TinCan;
using TinCan.LRSResponses;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// A preconfigured version of <see cref="QueuedLRS"/>.
    /// Automatically flushes the statement queue when the network status changes,
    /// an item is completed or bookmarked, and periodically.
    /// </summary>
    public class CachedLRS : QueuedLRS, ILRSService
    {
        const string StatementStoreFile = "statement_store.json";
        const string StateStoreFile = "state_resource_store.json";
        readonly ILRS targetLRS;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedLRS"/> class.
        /// </summary>
        public CachedLRS() : this(new LRSProxy())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedLRS"/> class.
        /// </summary>
        /// <param name="remoteLRS">The remote LRS.</param>
        public CachedLRS(ILRS remoteLRS) : base(remoteLRS, new JSONStatementStore(GetStorePath(StatementStoreFile)), new JSONStateResourceStore(GetStorePath(StateStoreFile)), GetTriggers())
        {
            this.targetLRS = remoteLRS;
        }

        /// <summary>
        /// Returns a list of all statements in the local cache.
        /// </summary>
        /// <returns>The list of cached statements.</returns>
        public IList<Statement> CachedStatements()
        {
            return new JSONStatementStore(GetStorePath(StatementStoreFile)).RestoreStatements();
        }

        /// <inheritdoc />
        public void UpdateEndpoint(Uri uri)
        {
            if (targetLRS is LRSProxy proxy && !proxy.CheckEndpoint(uri))
            {
                ClearQueue();
                proxy.UpdateEndpoint(uri);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            ClearQueue();
        }

        /// <inheritdoc />
        public string RawLocalCache()
        {
            var statementQueue = Queue.Select(s => s.ToJSON())?.Join("\n\n");
            if (statementQueue == string.Empty)
            {
                statementQueue = "none";
            }

            return statementQueue;
        }

        /// <inheritdoc />
        public async Task PersistQueuedStatements()
        {
            // Check if statement queue is empty
            if (QueueSize == 0)
            {
                return;
            }

            FlushStatementResult result;

            do
            {
                result = await FlushStatementQueueWithResponse().ConfigureAwait(false);
            }
            while (result != null && result.Response.success);

            // Check if statement queue is empty
            if (QueueSize > 0)
            {
                // If the queue is not empty, then there may still be statements representing
                // changes in the learner's state which need to be persisted.
                throw new SendStatementException(Strings.StatementSendError, result?.Response.httpException);
            }
        }

        /// <inheritdoc />
        public async Task PersistQueuedState()
        {
            IEnumerable<LRSResponse> responses;

            do
            {
                responses = await FlushStateResourceQueue().ConfigureAwait(false);
            }
            while (responses != null && responses?.FirstOrDefault()?.success == true);
        }

        static string GetStorePath(string filename)
        {
            return Path.Combine(DependencyService.Get<IPlatformFileProcessor>().NoBackupFolder, filename);
        }

        static IEnumerable<IQueueFlushTrigger> GetTriggers()
        {
            return new List<IQueueFlushTrigger>
            {
                new InternetConnectionTrigger(),
                new CompletedStatementTrigger(),
                new PeriodicTrigger(),
                new KeyEventTrigger(),
            };
        }

        /// <summary>
        /// Flushes the statement queue when a key PERLS event happens (i.e. bookmarking or responding to a prompt).
        /// </summary>
        internal class KeyEventTrigger : IQueueFlushTrigger
        {
            /// <inheritdoc />
            public event EventHandler TriggerFired;

            static IEnumerable<string> EventVerbs => new List<string>
            {
                Verbs.Favorited.id.OriginalString,
                Verbs.Unfavorited.id.OriginalString,
                Verbs.Responded.id.OriginalString,
                Verbs.Answered.id.OriginalString,
                Verbs.Voided.id.OriginalString,
                Verbs.Followed.id.OriginalString,
                Verbs.Unfollowed.id.OriginalString,
            };

            /// <inheritdoc />
            public void OnStatementQueued(Statement statement)
            {
                if (EventVerbs.Contains(statement.verb.id.OriginalString))
                {
                    TriggerFired?.Invoke(this, EventArgs.Empty);
                }
                else if (statement.result?.completion == true)
                {
                    TriggerFired?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
