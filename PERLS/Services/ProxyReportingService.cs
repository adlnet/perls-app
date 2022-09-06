using System;
using System.Collections.Generic;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ExperienceAPI;
using TinCan;

namespace PERLS.Services
{
    /// <summary>
    /// The catch-all reporting service.
    /// </summary>
    public class ProxyReportingService : IReportingService
    {
        /// <summary>
        /// The reporting services. Every call will be reported to each of these services, it is up to those services to determine if it is to be used.
        /// </summary>
        IEnumerable<IReportingService> reportingServices = new List<IReportingService>()
        {
            new AppCenterReportingService(),
            new ExperienceAPIReporting(),
        };

        /// <inheritdoc/>
        public void ReportAnnotationDeleted(IAnnotation annotation)
        {
            reportingServices.ForEach((arg) => arg.ReportAnnotationDeleted(annotation));
        }

        /// <inheritdoc/>
        public void ReportArticleFeedback(IRemoteResource resource, string webformId, int score, string feedback)
        {
            reportingServices.ForEach((arg) => arg.ReportArticleFeedback(resource, webformId, score, feedback));
        }

        /// <inheritdoc/>
        public void ReportArticleFeedbackByActivity(StatementTarget target, string webformId, int score, string feedback)
        {
            reportingServices.ForEach((arg) => arg.ReportArticleFeedbackByActivity(target, webformId, score, feedback));
        }

        /// <inheritdoc/>
        public void ReportAudioCompleted(IEpisode episode, TimeSpan length, double progress, TimeSpan time)
        {
            reportingServices.ForEach((arg) => arg.ReportAudioCompleted(episode, length, progress, time));
        }

        /// <inheritdoc/>
        public void ReportAudioInitialized(IEpisode episode, TimeSpan length, double speed)
        {
            reportingServices.ForEach((arg) => arg.ReportAudioInitialized(episode, length, speed));
        }

        /// <inheritdoc/>
        public void ReportAudioPaused(IEpisode episode, TimeSpan length, double progress, TimeSpan time)
        {
            reportingServices.ForEach((arg) => arg.ReportAudioPaused(episode, length, progress, time));
        }

        /// <inheritdoc/>
        public void ReportAudioPlayed(IEpisode episode, TimeSpan time)
        {
            reportingServices.ForEach((arg) => arg.ReportAudioPlayed(episode, time));
        }

        /// <inheritdoc/>
        public void ReportAudioSeeked(IEpisode episode, TimeSpan timeFrom, TimeSpan timeTo)
        {
            reportingServices.ForEach((arg) => arg.ReportAudioSeeked(episode, timeFrom, timeTo));
        }

        /// <inheritdoc/>
        public void ReportAudioTerminated(IEpisode episode, TimeSpan length, double progress, TimeSpan time)
        {
            reportingServices.ForEach((arg) => arg.ReportAudioTerminated(episode, length, progress, time));
        }

        /// <inheritdoc/>
        public void ReportAudioTerminated(IEpisode episode, TimeSpan length)
        {
            reportingServices.ForEach((arg) => arg.ReportAudioTerminated(episode, length));
        }

        /// <inheritdoc/>
        public void ReportBadgeSaved(IBadge badge)
        {
            reportingServices.ForEach((arg) => arg.ReportBadgeSaved(badge));
        }

        /// <inheritdoc/>
        public void ReportBadgeViewed(IBadge badge)
        {
            reportingServices.ForEach((arg) => arg.ReportBadgeViewed(badge));
        }

        /// <inheritdoc/>
        public void ReportCardFlipped(IItem item)
        {
            reportingServices.ForEach((arg) => arg.ReportCardFlipped(item));
        }

        /// <inheritdoc/>
        public void ReportCertificateShared(ICertificate certificate)
        {
            reportingServices.ForEach((arg) => arg.ReportCertificateShared(certificate));
        }

        /// <inheritdoc/>
        public void ReportCertificateViewed(ICertificate certificate)
        {
            reportingServices.ForEach((arg) => arg.ReportCertificateViewed(certificate));
        }

        /// <inheritdoc/>
        public void ReportCourseViewed(ICourse course)
        {
            reportingServices.ForEach((arg) => arg.ReportCourseViewed(course));
        }

        /// <inheritdoc/>
        public void ReportFileDownloaded(IFile file)
        {
            reportingServices.ForEach((arg) => arg.ReportFileDownloaded(file));
        }

        /// <inheritdoc/>
        public void ReportFileViewed(IFile file)
        {
            reportingServices.ForEach((arg) => arg.ReportFileViewed(file));
        }

        /// <inheritdoc/>
        public void ReportItemShared(IShareableRemoteResource shareable)
        {
            reportingServices.ForEach((arg) => arg.ReportItemShared(shareable));
        }

        /// <inheritdoc/>
        public void ReportPromptAnswered(IPrompt prompt, IPromptOption selectedOption)
        {
            reportingServices.ForEach((arg) => arg.ReportPromptAnswered(prompt, selectedOption));
        }

        /// <inheritdoc/>
        public void ReportPromptAsked(IPrompt prompt)
        {
            reportingServices.ForEach((arg) => arg.ReportPromptAsked(prompt));
        }

        /// <inheritdoc/>
        public void ReportPushNotificationSelected(Activity notification, string action)
        {
            reportingServices.ForEach((arg) => arg.ReportPushNotificationSelected(notification, action));
        }

        /// <inheritdoc/>
        public void ReportQuestionAnswered(IQuiz quiz, IQuizOption selectedOption, TimeSpan duration, ITest test = null)
        {
            reportingServices.ForEach((arg) => arg.ReportQuestionAnswered(quiz, selectedOption, duration, test));
        }

        /// <inheritdoc/>
        public void ReportQuestionAsked(IQuiz quiz, ITest test = null)
        {
            reportingServices.ForEach((arg) => arg.ReportQuestionAsked(quiz, test));
        }

        /// <inheritdoc/>
        public void ReportQuestionSkipped(IQuiz quiz)
        {
            reportingServices.ForEach((arg) => arg.ReportQuestionSkipped(quiz));
        }

        /// <inheritdoc/>
        public void ReportResourceCompleted(IRemoteResource resource)
        {
            reportingServices.ForEach((arg) => arg.ReportResourceCompleted(resource));
        }

        /// <inheritdoc/>
        public void ReportResourceViewed(IRemoteResource resource)
        {
            reportingServices.ForEach((arg) => arg.ReportResourceViewed(resource));
        }

        /// <inheritdoc/>
        public void ReportSearchQuery(string query)
        {
            reportingServices.ForEach((arg) => arg.ReportSearchQuery(query));
        }

        /// <inheritdoc/>
        public void ReportSearchResultSelected(string query, IRemoteResource resource)
        {
            reportingServices.ForEach((arg) => arg.ReportSearchResultSelected(query, resource));
        }

        /// <inheritdoc/>
        public void ReportTermViewed(ITaxonomyTerm term)
        {
            reportingServices.ForEach((arg) => arg.ReportTermViewed(term));
        }

        /// <inheritdoc/>
        public void ReportTestAttempted(ITest test)
        {
            reportingServices.ForEach((arg) => arg.ReportTestAttempted(test));
        }

        /// <inheritdoc/>
        public void ReportTestCompleted(ITest test, uint correctAnswers, TimeSpan duration = default)
        {
            reportingServices.ForEach((arg) => arg.ReportTestCompleted(test, correctAnswers, duration));
        }
    }
}
