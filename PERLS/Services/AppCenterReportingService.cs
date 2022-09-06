using System;
using System.Collections.Generic;
using System.Linq;
using Float.Core.Analytics;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using TinCan;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// The AppCenter Reporting Service.
    /// </summary>
    public class AppCenterReportingService : IReportingService
    {
        /// <inheritdoc/>
        public void ReportCourseViewed(ICourse course)
        {
            DependencyService.Get<AnalyticsService>().TrackEvent("Viewed Course", new Dictionary<string, string>
            {
                { "name", course?.Name },
            });
        }

        /// <inheritdoc/>
        public void ReportFileDownloaded(IFile file)
        {
            DependencyService.Get<AnalyticsService>().TrackEvent("File Downloaded", new Dictionary<string, string>
            {
                { "fileName", file?.Name },
            });
        }

        /// <inheritdoc/>
        public void ReportFileViewed(IFile file)
        {
            DependencyService.Get<AnalyticsService>().TrackEvent("File Viewed", new Dictionary<string, string>
            {
                { "fileName", file?.Name },
            });
        }

        /// <inheritdoc/>
        public void ReportItemShared(IShareableRemoteResource shareable)
        {
            DependencyService.Get<AnalyticsService>().TrackEvent("File Shared", new Dictionary<string, string>
            {
                { "fileName", shareable?.Name },
            });
        }

        /// <inheritdoc/>
        public void ReportResourceCompleted(IRemoteResource resource)
        {
            DependencyService.Get<AnalyticsService>().TrackEvent("Resource Completed", new Dictionary<string, string>
            {
                { "resource", resource?.Name },
            });
        }

        /// <inheritdoc/>
        public void ReportResourceViewed(IRemoteResource resource)
        {
            DependencyService.Get<AnalyticsService>().TrackEvent("Resource Viewed", new Dictionary<string, string>
            {
                { "resource", resource?.Name },
            });
        }

        /// <inheritdoc/>
        public void ReportSearchQuery(string query)
        {
            DependencyService.Get<AnalyticsService>().TrackEvent("Searched", new Dictionary<string, string>
            {
                { "query", query },
            });
        }

        /// <inheritdoc/>
        public void ReportSearchResultSelected(string query, IRemoteResource resource)
        {
            DependencyService.Get<AnalyticsService>().TrackEvent("Search Item Selected", new Dictionary<string, string>
            {
                { "query", query },
                { "selectedItem", resource?.Name },
            });
        }

        /// <inheritdoc/>
        public void ReportTestCompleted(ITest test, uint correctAnswers, TimeSpan duration = default)
        {
            var totalQuestions = test?.Questions?.Count() ?? 0;
            var success = correctAnswers / (float)totalQuestions >= test?.PercentRequiredToPass;

            DependencyService.Get<AnalyticsService>().TrackEvent("Test Completed", new Dictionary<string, string>
            {
                { "name", test?.Name },
                { "success", $"{success}" },
            });
        }

        /// <inheritdoc/>
        public void ReportAnnotationDeleted(IAnnotation annotation)
        {
        }

        /// <inheritdoc/>
        public void ReportArticleFeedback(IRemoteResource resource, string webformId, int score, string feedback)
        {
        }

        /// <inheritdoc/>
        public void ReportArticleFeedbackByActivity(StatementTarget target, string webformId, int score, string feedback)
        {
        }

        /// <inheritdoc/>
        public void ReportAudioCompleted(IEpisode episode, TimeSpan length, double progress, TimeSpan time)
        {
        }

        /// <inheritdoc/>
        public void ReportAudioInitialized(IEpisode episode, TimeSpan length, double speed)
        {
        }

        /// <inheritdoc/>
        public void ReportAudioPaused(IEpisode episode, TimeSpan length, double progress, TimeSpan time)
        {
        }

        /// <inheritdoc/>
        public void ReportAudioPlayed(IEpisode episode, TimeSpan time)
        {
        }

        /// <inheritdoc/>
        public void ReportAudioSeeked(IEpisode episode, TimeSpan timeFrom, TimeSpan timeTo)
        {
        }

        /// <inheritdoc/>
        public void ReportAudioTerminated(IEpisode episode, TimeSpan length, double progress, TimeSpan time)
        {
        }

        /// <inheritdoc/>
        public void ReportAudioTerminated(IEpisode episode, TimeSpan length)
        {
        }

        /// <inheritdoc/>
        public void ReportBadgeSaved(IBadge badge)
        {
        }

        /// <inheritdoc/>
        public void ReportBadgeViewed(IBadge badge)
        {
        }

        /// <inheritdoc/>
        public void ReportCardFlipped(IItem item)
        {
        }

        /// <inheritdoc/>
        public void ReportCertificateShared(ICertificate certificate)
        {
        }

        /// <inheritdoc/>
        public void ReportCertificateViewed(ICertificate certificate)
        {
        }

        /// <inheritdoc/>
        public void ReportPromptAnswered(IPrompt prompt, IPromptOption selectedOption)
        {
        }

        /// <inheritdoc/>
        public void ReportPromptAsked(IPrompt prompt)
        {
        }

        /// <inheritdoc/>
        public void ReportPushNotificationSelected(Activity notification, string action)
        {
        }

        /// <inheritdoc/>
        public void ReportQuestionAnswered(IQuiz quiz, IQuizOption selectedOption, TimeSpan duration, ITest test = null)
        {
        }

        /// <inheritdoc/>
        public void ReportQuestionAsked(IQuiz quiz, ITest test = null)
        {
        }

        /// <inheritdoc/>
        public void ReportQuestionSkipped(IQuiz quiz)
        {
        }

        /// <inheritdoc/>
        public void ReportTermViewed(ITaxonomyTerm term)
        {
        }

        /// <inheritdoc/>
        public void ReportTestAttempted(ITest test)
        {
        }
    }
}
