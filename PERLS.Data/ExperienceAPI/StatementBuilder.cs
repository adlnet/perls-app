using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using TinCan;
using Xamarin.Forms;

namespace PERLS.Data.ExperienceAPI
{
    /// <summary>
    /// Convenience methods for building xAPI statements.
    /// </summary>
    internal class StatementBuilder
    {
        readonly Statement statement;
        readonly IAppContextService appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatementBuilder"/> class.
        /// </summary>
        protected StatementBuilder() : this(new Statement(), DependencyService.Get<IAppContextService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatementBuilder"/> class.
        /// </summary>
        /// <param name="statement">An existing xAPI statement to edit.</param>
        /// <param name="appContext">The application context.</param>
        protected StatementBuilder(Statement statement, IAppContextService appContext)
        {
            this.statement = statement;
            this.appContext = appContext;
        }

        /// <summary>
        /// Creates a new statement with the actor being the current learner.
        /// </summary>
        /// <returns>A <see cref="StatementBuilder"/> object.</returns>
        public static StatementBuilder LearnerActor()
        {
            return Create().SetActorToLearner();
        }

        /// <summary>
        /// Creates a new statement with the actor being the application/system.
        /// </summary>
        /// <returns>A <see cref="StatementBuilder"/> object.</returns>
        public static StatementBuilder SystemActor()
        {
            return Create().SetActorToSystem();
        }

        /// <summary>
        /// Creates a new <see cref="StatementBuilder"/> object.
        /// </summary>
        /// <returns>A <see cref="StatementBuilder"/> object.</returns>
        public static StatementBuilder Create()
        {
            return new StatementBuilder();
        }

        public Agent GetAgent()
        {
            return statement.actor;
        }

        public StatementBuilder SetActorToLearner()
        {
            return SetActor(appContext.LearnerAgent);
        }

        public StatementBuilder SetActorToSystem()
        {
            return SetActor(appContext.SystemAgent);
        }

        public Statement GetStatement()
        {
            statement.Stamp();
            return statement;
        }

        public StatementBuilder SetActor(Agent actor)
        {
            statement.actor = actor;
            return this;
        }

        public StatementBuilder SetVerb(Verb verb)
        {
            statement.verb = verb;
            return this;
        }

        public StatementBuilder SetObject(IRemoteResource resource)
        {
            statement.target = ActivityBuilder.FromResource(resource);

            if (resource is IItem item && item.Topic is IRemoteResource remoteResource)
            {
                AddGroupingActivity(remoteResource);
            }

            return this;
        }

        public StatementBuilder SetObject(StatementTarget target)
        {
            statement.target = target;
            return this;
        }

        public StatementBuilder SetResponse(string response)
        {
            var result = PrepareResult();
            result.response = response;

            return this;
        }

        public StatementBuilder AddResult(bool completion = true, bool success = true, string response = null)
        {
            var result = PrepareResult();
            result.completion = completion;
            result.success = success;
            result.response = response;

            return this;
        }

        public StatementBuilder AddDuration(TimeSpan duration)
        {
            PrepareResult().duration = duration;

            return this;
        }

        public StatementBuilder AddScore(double value, double? min = null, double? max = null, double? scaled = null)
        {
            if (scaled == null && max != null && max != 0 && max != min)
            {
                if (min != null)
                {
                    scaled = value / (max - min);
                }
                else
                {
                    scaled = value / max;
                }
            }

            var score = new Score
            {
                scaled = scaled,
                raw = value,
                min = min,
                max = max,
            };

            PrepareResult().score = score;

            return this;
        }

        public StatementBuilder AddGroupingActivity(IRemoteResource resource)
        {
            if (resource == null)
            {
                return this;
            }

            var activity = ActivityBuilder.FromResource(resource);
            return AddGroupingActivity(activity);
        }

        public StatementBuilder AddGroupingActivity(Activity activity)
        {
            var contextActivities = PrepareContextActivities();

            if (contextActivities.grouping == null)
            {
                contextActivities.grouping = new List<Activity>();
            }

            contextActivities.grouping.Add(activity);

            return this;
        }

        public StatementBuilder AddParentActivity(IRemoteResource resource)
        {
            if (resource == null)
            {
                return this;
            }

            var activity = ActivityBuilder.FromResource(resource);
            return AddParentActivity(activity);
        }

        public StatementBuilder AddParentActivity(Activity activity)
        {
            var contextActivities = PrepareContextActivities();

            if (contextActivities.parent == null)
            {
                contextActivities.parent = new List<Activity>();
            }

            contextActivities.parent.Add(activity);

            return this;
        }

        public StatementBuilder AddResultExtension(Uri uri, JToken value)
        {
            var result = PrepareResult();

            Dictionary<Uri, JToken> extensions = new Dictionary<Uri, JToken>();
            if (result.extensions != null)
            {
                extensions = JsonConvert.DeserializeObject<Dictionary<Uri, JToken>>(result.extensions.ToJSON());
            }

            extensions.Add(uri, value);
            result.extensions = new TinCan.Extensions(JObject.FromObject(extensions));
            return this;
        }

        public StatementBuilder SetRegistration(Guid guid)
        {
            PrepareContext().registration = guid;
            return this;
        }

        ContextActivities PrepareContextActivities()
        {
            PrepareContext();

            if (statement.context.contextActivities == null)
            {
                statement.context.contextActivities = new ContextActivities();
            }

            return statement.context.contextActivities;
        }

        Context PrepareContext()
        {
            if (statement.context == null)
            {
                statement.context = new Context();
            }

            return statement.context;
        }

        Result PrepareResult()
        {
            if (statement.result == null)
            {
                statement.result = new Result();
            }

            return statement.result;
        }
    }
}
