using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Float.Core.Net;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.DataImplementation.Models;
using Xamarin.Forms;

namespace PERLS.DataImplementation.Providers
{
    /// <summary>
    /// The Learning Provider implementation.
    /// </summary>
    public class LearnerProvider : ILearnerProvider, IRemoteProvider
    {
        readonly DrupalAPI drupalAPI;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerProvider"/> class.
        /// </summary>
        public LearnerProvider()
        {
            drupalAPI = DependencyService.Get<DrupalAPI>();
        }

        /// <inheritdoc />
        public Task<bool> IsReachable() => drupalAPI.IsReachable();

        /// <inheritdoc />
        public Task<ILearner> GetCurrentLearner() => drupalAPI.GetData<Learner, ILearner>("api/user/me");

        /// <inheritdoc />
        public Task<ILearnerStats> GetCurrentLearnerStats() => drupalAPI.GetData<LearnerStats, ILearnerStats>("api/stats");

        /// <inheritdoc/>
        public Task<IEnumerable<IPrompt>> GetPrompts() => drupalAPI.GetData<IEnumerable<Prompt>, IEnumerable<IPrompt>>("api/prompts");

        /// <inheritdoc/>
        public Task<IEnumerable<ICertificate>> GetCertificates() => drupalAPI.GetData<IEnumerable<Certificate>, IEnumerable<ICertificate>>("api/certificates");

        /// <inheritdoc/>
        public Task<ICertificate> GetCertificateItemFromId(string itemId) => throw new NotImplementedException();

        /// <inheritdoc/>
        public Task<IEnumerable<IBadge>> GetBadges() => drupalAPI.GetData<IEnumerable<Badge>, IEnumerable<IBadge>>("api/badges");

        /// <inheritdoc/>
        public Task<IBadge> GetBadgeItemFromId(string itemId) => throw new NotImplementedException();

        /// <inheritdoc/>
        public async Task SaveCurrentLearnerGoals()
        {
            var user = DependencyService.Get<IAppContextService>().CurrentLearner;

            // Create the json for the goals.
            var query = new Dictionary<string, object>
            {
                { "id", user.Id },
            };

            var goals = new Dictionary<string, object>
            {
                { "weekly_test_average", user.LearnerGoals.AverageTestScoreGoal },
                { "monthly_course_completions", user.LearnerGoals.CoursesCompletedPerMonthGoal },
                { "weekly_completions", user.LearnerGoals.ArticlesCompletedPerWeekGoal },
                { "weekly_views", user.LearnerGoals.ArticlesViewedPerWeekGoal },
                { "notification_time", user.LearnerGoals.ReminderTime },
                { "notification_days", user.LearnerGoals.ReminderDaysServerString },
            };

            query.Add("goals", goals);

            try
            {
                _ = await drupalAPI.Patch("api/user/me", query).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                DependencyService.Get<INotificationHandler>().NotifyException(e);
            }
        }
    }
}
