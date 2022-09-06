using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Analytics;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ExperienceAPI.Profiles;
using PERLS.Data.Extensions;
using TinCan;
using TinCan.LRSResponses;
using Xamarin.Forms;

namespace PERLS.Data.ExperienceAPI
{
    /// <summary>
    /// Reporting service that reports activity to an LRS.
    /// </summary>
    public class ExperienceAPIReporting : IReportingService
    {
        readonly ILRSService lrsService = DependencyService.Get<ILRSService>();
        readonly Dictionary<Guid, Guid> registrations = new Dictionary<Guid, Guid>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExperienceAPIReporting"/> class.
        /// </summary>
        public ExperienceAPIReporting()
        {
        }

        /// <inheritdoc />
        public void ReportTermViewed(ITaxonomyTerm term)
        {
            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Viewed)
                .SetObject(term)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportCardFlipped(IItem item)
        {
            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Interacted)
                .SetObject(item)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportCourseViewed(ICourse course)
        {
            if (course == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Launched)
                .SetObject(course)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportTestAttempted(ITest test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            if (registrations.ContainsKey(test.Id))
            {
                registrations.Remove(test.Id);
            }

            var registration = Guid.NewGuid();
            registrations.Add(test.Id, registration);

            var testStatement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Assessment.Verbs.Attempted)
                .SetObject(test)
                .SetRegistration(registration)
                .GetStatement();

            var statements = new List<Statement>
            {
                testStatement,
            };

            SendStatements(statements);
        }

        /// <inheritdoc />
        public void ReportTestCompleted(ITest test, uint correctAnswers, TimeSpan duration = default)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            if (!registrations.ContainsKey(test.Id))
            {
                return;
            }

            var registration = registrations[test.Id];

            var totalQuestions = test.Questions.Count();
            var success = correctAnswers / (float)totalQuestions >= test.PercentRequiredToPass;

            // Using the topic from the questions is a work around because tests dont have a topic right now.
            var grouping = test.Questions?.FirstOrDefault()?.Topic;

            var testStatement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Assessment.Verbs.Completed)
                .SetObject(test)
                .AddDuration(duration)
                .AddGroupingActivity(grouping)
                .SetRegistration(registration)
                .AddResult(true, success)
                .GetStatement();

            var resultStatement = StatementBuilder
                .LearnerActor()
                .SetVerb(success ? Profiles.Cmi5.Verbs.Passed : Profiles.Cmi5.Verbs.Failed)
                .AddResult(true, success)
                .SetObject(test)
                .AddGroupingActivity(grouping)
                .SetRegistration(registration)
                .AddScore(correctAnswers, max: totalQuestions)
                .AddDuration(duration)
                .GetStatement();

            var statements = new List<Statement>
            {
                testStatement,
                resultStatement,
            };

            registrations.Remove(test.Id);
            SendStatements(statements);
        }

        /// <inheritdoc />
        public void ReportQuestionAsked(IQuiz quiz, ITest test = null)
        {
            if (quiz == null)
            {
                throw new ArgumentNullException(nameof(quiz));
            }

            Guid registration;

            if (test != null)
            {
                // If this question is part of a test,
                // then the registration must be defined with the test.
                if (!registrations.ContainsKey(test.Id))
                {
                    return;
                }

                registration = registrations[test.Id];
            }
            else
            {
                // Otherwise, if this question is being asked on it's own,
                // then a new registration must be created.
                if (registrations.ContainsKey(quiz.Id))
                {
                    return;
                }

                registration = Guid.NewGuid();
                registrations.Add(quiz.Id, registration);
            }

            var statements = new List<Statement>();

            // When a quiz question is not part of a test,
            // then it is considered an assessment unto itself.
            if (test == null)
            {
                var attemptedStatement = StatementBuilder
                    .LearnerActor()
                    .SetVerb(Profiles.Assessment.Verbs.Attempted)
                    .SetObject(ActivityBuilder.QuizAsAssessment(quiz))
                    .AddGroupingActivity(quiz.Topic)
                    .SetRegistration(registration)
                    .GetStatement();
                statements.Add(attemptedStatement);
            }

            var askedStatement = StatementBuilder
                .SystemActor()
                .SetVerb(Profiles.Assessment.Verbs.Asked)
                .SetObject(quiz)
                .AddParentActivity(test != null ? ActivityBuilder.FromResource(test) : ActivityBuilder.QuizAsAssessment(quiz))
                .GetStatement();
            statements.Add(askedStatement);

            SendStatements(statements);
        }

        /// <inheritdoc />
        public void ReportQuestionAnswered(IQuiz quiz, IQuizOption selectedOption, TimeSpan duration = default, ITest test = null)
        {
            if (quiz == null)
            {
                throw new ArgumentNullException(nameof(quiz));
            }

            if (selectedOption == null)
            {
                throw new ArgumentNullException(nameof(selectedOption));
            }

            var assessment = test != null ? (IRemoteResource)test : quiz;
            if (!registrations.ContainsKey(assessment.Id))
            {
                return;
            }

            var registration = registrations[assessment.Id];

            var statements = new List<Statement>();

            var answeredStatement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Assessment.Verbs.Answered)
                .SetObject(quiz)
                .AddParentActivity(test != null ? ActivityBuilder.FromResource(test) : ActivityBuilder.QuizAsAssessment(quiz))
                .AddGroupingActivity(test != null ? test.Questions?.FirstOrDefault()?.Topic : quiz.Topic)
                .AddResult(success: selectedOption.IsCorrect, response: selectedOption.Text)
                .SetRegistration(registration)
                .AddScore(selectedOption.IsCorrect ? 1 : 0, max: 1)
                .AddDuration(duration)
                .GetStatement();

            statements.Add(answeredStatement);

            // If the question was not part of a test, then consider the assessment complete too.
            if (test == null)
            {
                var completedStatement = StatementBuilder
                    .LearnerActor()
                    .SetVerb(Profiles.Assessment.Verbs.Completed)
                    .AddGroupingActivity(quiz.Topic)
                    .SetObject(ActivityBuilder.QuizAsAssessment(quiz))
                    .AddResult(success: selectedOption.IsCorrect)
                    .SetRegistration(registration)
                    .AddDuration(duration)
                    .GetStatement();

                var resultVerb = selectedOption.IsCorrect ? Profiles.Cmi5.Verbs.Passed : Profiles.Cmi5.Verbs.Failed;
                var resultStatement = StatementBuilder
                    .LearnerActor()
                    .SetVerb(resultVerb)
                    .AddGroupingActivity(quiz.Topic)
                    .SetObject(ActivityBuilder.QuizAsAssessment(quiz))
                    .AddScore(selectedOption.IsCorrect ? 1 : 0, max: 1)
                    .AddDuration(duration)
                    .GetStatement();

                statements.Add(completedStatement);
                statements.Add(resultStatement);
            }

            registrations.Remove(quiz.Id);

            SendStatements(statements);
        }

        /// <inheritdoc />
        public void ReportQuestionSkipped(IQuiz quiz)
        {
            if (quiz == null)
            {
                throw new ArgumentNullException(nameof(quiz));
            }

            registrations.Remove(quiz.Id);

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Skipped)
                .SetObject(quiz)
                .AddParentActivity(ActivityBuilder.QuizAsAssessment(quiz))
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportSearchQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Searched)
                .SetObject(ActivityBuilder.FromApplication())
                .SetResponse(query)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportSearchResultSelected(string query, IRemoteResource resource)
        {
            if (resource == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Selected)
                .SetObject(resource)
                .SetResponse(query)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportResourceViewed(IRemoteResource resource)
        {
            if (resource == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Viewed)
                .SetObject(resource)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportResourceCompleted(IRemoteResource resource)
        {
            if (resource == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Cmi5.Verbs.Completed)
                .SetObject(resource)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportFileDownloaded(IFile file)
        {
            if (file == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Downloaded)
                .SetObject(ActivityBuilder.FromFile(file))
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportFileViewed(IFile file)
        {
            if (file == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Viewed)
                .SetObject(ActivityBuilder.FromFile(file))
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc/>
        public void ReportPromptAsked(IPrompt prompt)
        {
            var statement = StatementBuilder
                .SystemActor()
                .SetVerb(Profiles.Assessment.Verbs.Asked)
                .SetObject(ActivityBuilder.FromPrompt(prompt))
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc/>
        public void ReportPromptAnswered(IPrompt prompt, IPromptOption selectedOption)
        {
            if (selectedOption == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Responded)
                .SetObject(ActivityBuilder.FromPrompt(prompt))
                .SetResponse(selectedOption.Key)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportArticleFeedback(IRemoteResource resource, string webformId, int score, string feedback)
        {
            if (resource == null || score == 0)
            {
                return;
            }

            var uri = new Uri("http://xapi.gowithfloat.net/extension/form-id");
            Statement statement;

            if (score > 0)
            {
                statement = StatementBuilder
                    .LearnerActor()
                    .SetVerb(Profiles.Perls.Verbs.VotedUp)
                    .SetObject(resource)
                    .SetResponse(feedback)
                    .AddResultExtension(uri, webformId)
                    .GetStatement();
            }
            else
            {
                statement = StatementBuilder
                    .LearnerActor()
                    .SetVerb(Profiles.Perls.Verbs.VotedDown)
                    .SetObject(resource)
                    .SetResponse(feedback)
                    .AddResultExtension(uri, webformId)
                    .GetStatement();
            }

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportArticleFeedbackByActivity(StatementTarget target, string webformId, int score, string feedback)
        {
            if (target == null || score == 0)
            {
                return;
            }

            var uri = new Uri("http://xapi.gowithfloat.net/extension/form-id");
            Statement statement = StatementBuilder
                .LearnerActor()
                .SetVerb(score > 0 ? Profiles.Perls.Verbs.VotedUp : Profiles.Perls.Verbs.VotedDown)
                .SetObject(target)
                .SetResponse(feedback)
                .AddResultExtension(uri, webformId)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc/>
        public void ReportAnnotationDeleted(IAnnotation annotation)
        {
            if (annotation == null)
            {
                return;
            }

            lrsService.VoidStatement(new Guid(annotation.StatementId), StatementBuilder.LearnerActor().GetAgent()).OnFailure(task =>
            {
                DependencyService.Get<AnalyticsService>().TrackException(task.Exception);
            });
        }

        /// <inheritdoc />
        public void ReportPushNotificationSelected(Activity notification, string action)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Interacted)
                .SetObject(notification)
                .AddGroupingActivity(ActivityBuilder.NotificationGroup(action))
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportAudioInitialized(IEpisode episode, TimeSpan length, double speed)
        {
            if (episode == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Audio.Verbs.Initialized)
                .SetObject(ActivityBuilder.FromEpisode(episode))
                .AddResultExtension(Audio.Extensions.Length, length.TotalSeconds.ToRoundedHundredths())
                .AddResultExtension(Audio.Extensions.Speed, speed)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportAudioCompleted(IEpisode episode, TimeSpan length, double progress, TimeSpan time)
        {
            if (episode == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Audio.Verbs.Completed)
                .SetObject(ActivityBuilder.FromEpisode(episode))
                .AddResultExtension(Audio.Extensions.Length, length.TotalSeconds.ToRoundedHundredths())
                .AddResultExtension(Audio.Extensions.Progress, progress.ToRoundedHundredths())
                .AddResultExtension(Audio.Extensions.Time, time.TotalSeconds.ToRoundedHundredths())
                .AddDuration(length)
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportAudioPlayed(IEpisode episode, TimeSpan time)
        {
            if (episode == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Audio.Verbs.Played)
                .SetObject(ActivityBuilder.FromEpisode(episode))
                .AddResultExtension(Audio.Extensions.Time, time.TotalSeconds.ToRoundedHundredths())
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportAudioPaused(IEpisode episode, TimeSpan length, double progress, TimeSpan time)
        {
            if (episode == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Audio.Verbs.Paused)
                .SetObject(ActivityBuilder.FromEpisode(episode))
                .AddResultExtension(Audio.Extensions.Length, length.TotalSeconds.ToRoundedHundredths())
                .AddResultExtension(Audio.Extensions.Progress, progress.ToRoundedHundredths())
                .AddResultExtension(Audio.Extensions.Time, time.TotalSeconds.ToRoundedHundredths())
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportAudioSeeked(IEpisode episode, TimeSpan timeFrom, TimeSpan timeTo)
        {
            if (episode == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Audio.Verbs.Seeked)
                .SetObject(ActivityBuilder.FromEpisode(episode))
                .AddResultExtension(Audio.Extensions.TimeFrom, timeFrom.TotalSeconds.ToRoundedHundredths())
                .AddResultExtension(Audio.Extensions.TimeTo, timeTo.TotalSeconds.ToRoundedHundredths())
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportAudioTerminated(IEpisode episode, TimeSpan length, double progress, TimeSpan time)
        {
            if (episode == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Audio.Verbs.Terminated)
                .SetObject(ActivityBuilder.FromEpisode(episode))
                .AddResultExtension(Audio.Extensions.Length, length.TotalSeconds.ToRoundedHundredths())
                .AddResultExtension(Audio.Extensions.Progress, progress.ToRoundedHundredths())
                .AddResultExtension(Audio.Extensions.Time, time.TotalSeconds.ToRoundedHundredths())
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc />
        public void ReportAudioTerminated(IEpisode episode, TimeSpan length)
        {
            if (episode == null)
            {
                return;
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Audio.Verbs.Terminated)
                .SetObject(ActivityBuilder.FromEpisode(episode))
                .AddResultExtension(Audio.Extensions.Length, length.TotalSeconds.ToRoundedHundredths())
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc/>
        public void ReportCertificateViewed(ICertificate certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Viewed)
                .SetObject(ActivityBuilder.FromCertificate(certificate))
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc/>
        public void ReportCertificateShared(ICertificate certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Shared)
                .SetObject(ActivityBuilder.FromCertificate(certificate))
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc/>
        public void ReportBadgeViewed(IBadge badge)
        {
            if (badge == null)
            {
                throw new ArgumentNullException(nameof(badge));
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Viewed)
                .SetObject(ActivityBuilder.FromBadge(badge))
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc/>
        public void ReportBadgeSaved(IBadge badge)
        {
            if (badge == null)
            {
                throw new ArgumentNullException(nameof(badge));
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Shared)
                .SetObject(ActivityBuilder.FromBadge(badge))
                .GetStatement();

            SendStatement(statement);
        }

        /// <inheritdoc/>
        public void ReportItemShared(IShareableRemoteResource shareable)
        {
            if (shareable == null)
            {
                throw new ArgumentNullException(nameof(shareable));
            }

            var statement = StatementBuilder
                .LearnerActor()
                .SetVerb(Profiles.Perls.Verbs.Shared)
                .SetObject(ActivityBuilder.FromResource(shareable))
                .GetStatement();

            SendStatement(statement);
        }

        void SendStatement(Statement statement)
        {
            var statements = new List<Statement> { statement };
            SendStatements(statements);
        }

        void SendStatements(List<Statement> statements)
        {
            SendStatementsAsync(statements)
                .OnFailure(task =>
                {
                    DependencyService.Get<AnalyticsService>().TrackException(task.Exception);
                });
        }

        /// <summary>
        /// Sends a statement to the LRS.
        /// </summary>
        /// <param name="statements">The statements to send.</param>
        /// <returns>The result of sending the statements.</returns>
        async Task<StatementsResultLRSResponse> SendStatementsAsync(List<Statement> statements)
        {
            var result = await lrsService.SaveStatements(statements).ConfigureAwait(false);

            if (result.httpException != null)
            {
                throw result.httpException;
            }

            if (!result.success)
            {
                throw new Exception(result.errMsg);
            }

            return result;
        }
    }
}
