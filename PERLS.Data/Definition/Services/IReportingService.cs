using System;
using TinCan;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// Defines a service for reporting learner behavior and app activity.
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// The user viewed a taxonomy term.
        /// </summary>
        /// <param name="term">The term viewed.</param>
        void ReportTermViewed(ITaxonomyTerm term);

        /// <summary>
        /// A card was flipped over.
        /// </summary>
        /// <param name="item">The content represented by the card.</param>
        void ReportCardFlipped(IItem item);

        /// <summary>
        /// The learner viewed a course.
        /// </summary>
        /// <param name="course">The course.</param>
        void ReportCourseViewed(ICourse course);

        /// <summary>
        /// The learner viewed a test.
        /// </summary>
        /// <param name="test">The test.</param>
        void ReportTestAttempted(ITest test);

        /// <summary>
        /// The learner completed a test.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="correctAnswers">The correct number of answers.</param>
        /// <param name="duration">The duration, optionally.</param>
        void ReportTestCompleted(ITest test, uint correctAnswers, TimeSpan duration = default);

        /// <summary>
        /// A question was asked of the learner.
        /// </summary>
        /// <param name="quiz">The quiz question presented to the learner.</param>
        /// <param name="test">The containing test, optionally.</param>
        void ReportQuestionAsked(IQuiz quiz, ITest test = null);

        /// <summary>
        /// The learner answered a quiz question.
        /// </summary>
        /// <param name="quiz">The quiz question the learner answered.</param>
        /// <param name="selectedOption">The quiz option selected by the learner.</param>
        /// <param name="duration">The time it took for the learner to answer the question.</param>
        /// <param name="test">The containing test, optionally.</param>
        void ReportQuestionAnswered(IQuiz quiz, IQuizOption selectedOption, TimeSpan duration, ITest test = null);

        /// <summary>
        /// The learner skipped a quiz (they saw the quiz question but did not answer it).
        /// </summary>
        /// <param name="quiz">The quiz that the learner skipped.</param>
        void ReportQuestionSkipped(IQuiz quiz);

        /// <summary>
        /// The learner performed a search query to find content.
        /// </summary>
        /// <param name="query">The learner's search query.</param>
        void ReportSearchQuery(string query);

        /// <summary>
        /// The learner selected a search result after searching for content.
        /// </summary>
        /// <param name="query">The learner's search query.</param>
        /// <param name="resource">The resource the learner selected.</param>
        void ReportSearchResultSelected(string query, IRemoteResource resource);

        /// <summary>
        /// The learner viewed a generic resource.
        /// </summary>
        /// <param name="resource">The resource viewed.</param>
        void ReportResourceViewed(IRemoteResource resource);

        /// <summary>
        /// The learner completed a generic resource.
        /// </summary>
        /// <param name="resource">The completed resource.</param>
        void ReportResourceCompleted(IRemoteResource resource);

        /// <summary>
        /// The learner downloaded a file.
        /// </summary>
        /// <param name="file">The downloaded file.</param>
        void ReportFileDownloaded(IFile file);

        /// <summary>
        /// The learner viewed a vile.
        /// </summary>
        /// <param name="file">The file that was viewed.</param>
        void ReportFileViewed(IFile file);

        /// <summary>
        /// The learner was presented with a prompt.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        void ReportPromptAsked(IPrompt prompt);

        /// <summary>
        /// The learner answered a prompt.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <param name="selectedOption">The selected option.</param>
        void ReportPromptAnswered(IPrompt prompt, IPromptOption selectedOption);

        /// <summary>
        /// The learner wrote feedback on an article.
        /// </summary>
        /// <param name="resource">The resouce that was given feedback.</param>
        /// <param name="webformId">The WebformId of the form.</param>
        /// <param name="score">The score the user gave the article.</param>
        /// <param name="feedback">The feedback given by the user.</param>
        void ReportArticleFeedback(IRemoteResource resource, string webformId, int score, string feedback);

        /// <summary>
        /// Sends article feedback based on activity.
        /// </summary>
        /// <param name="target">The activity.</param>
        /// <param name="webformId">The form id.</param>
        /// <param name="score">The user provided score.</param>
        /// <param name="feedback">The user provided feedback.</param>
        void ReportArticleFeedbackByActivity(StatementTarget target, string webformId, int score, string feedback);

        /// <summary>
        /// The learner deleted an annotation.
        /// </summary>
        /// <param name="annotation">The annotation to be deleted.</param>
        void ReportAnnotationDeleted(IAnnotation annotation);

        /// <summary>
        /// The user tapped on a push notification.
        /// </summary>
        /// <param name="notification">The notification tapped on.</param>
        /// <param name="action">The action of the notification.</param>
        void ReportPushNotificationSelected(Activity notification, string action);

        /// <summary>
        /// The media player has the episode current in the queue to play.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="length">The length.</param>
        /// <param name="speed">The speed.</param>
        void ReportAudioInitialized(IEpisode episode, TimeSpan length, double speed);

        /// <summary>
        /// The media player completed playback on the episode.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="length">The length.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="time">The time.</param>
        void ReportAudioCompleted(IEpisode episode, TimeSpan length, double progress, TimeSpan time);

        /// <summary>
        /// The media player starting playing the episode.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="time">The time.</param>
        void ReportAudioPlayed(IEpisode episode, TimeSpan time);

        /// <summary>
        /// The media player paused playing the episode.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="length">The length.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="time">The time.</param>
        void ReportAudioPaused(IEpisode episode, TimeSpan length, double progress, TimeSpan time);

        /// <summary>
        /// The learner finished dragging the scrubber during media playback.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="timeFrom">The time the user dragged from.</param>
        /// <param name="timeTo">The time the user dragged to.</param>
        void ReportAudioSeeked(IEpisode episode, TimeSpan timeFrom, TimeSpan timeTo);

        /// <summary>
        /// The media has been terminated with the episode current in the queue.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="length">The length.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="time">The time.</param>
        void ReportAudioTerminated(IEpisode episode, TimeSpan length, double progress, TimeSpan time);

        /// <summary>
        /// The media has been terminated with the episode current in the queue.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="length">The length.</param>
        void ReportAudioTerminated(IEpisode episode, TimeSpan length);

        /// <summary>
        /// The learner viewed the certificate.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        void ReportCertificateViewed(ICertificate certificate);

        /// <summary>
        /// The learner shared the certificate.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        void ReportCertificateShared(ICertificate certificate);

        /// <summary>
        /// The learner viewed the badge.
        /// </summary>
        /// <param name="badge">The badge.</param>
        void ReportBadgeViewed(IBadge badge);

        /// <summary>
        /// The learner saved the badge.
        /// </summary>
        /// <param name="badge">The badge.</param>
        void ReportBadgeSaved(IBadge badge);

        /// <summary>
        /// The learner shared a resource.
        /// </summary>
        /// <param name="shareable">the shareable resource.</param>
        void ReportItemShared(IShareableRemoteResource shareable);
    }
}
