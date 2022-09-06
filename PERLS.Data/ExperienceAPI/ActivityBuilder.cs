using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ExperienceAPI.Profiles.Perls;
using TinCan;
using Xamarin.Forms;

namespace PERLS.Data.ExperienceAPI
{
    /// <summary>
    /// Convenience methods for creating an xAPI activity.
    /// </summary>
    public class ActivityBuilder
    {
        readonly Activity activity;
        readonly IAppContextService appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityBuilder"/> class.
        /// </summary>
        protected ActivityBuilder() : this(new Activity(), DependencyService.Get<IAppContextService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityBuilder"/> class.
        /// </summary>
        /// <param name="activity">An existing xAPI activity (which will be edited).</param>
        /// <param name="appContext">The application context.</param>
        protected ActivityBuilder(Activity activity, IAppContextService appContext)
        {
            this.activity = activity;
            this.appContext = appContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityBuilder"/> class.
        /// </summary>
        /// <param name="id">The ID of the activity.</param>
        /// <param name="definition">The definition of the activity.</param>
        /// <param name="appContext">The application context.</param>
        protected ActivityBuilder(Uri id, ActivityDefinition definition, IAppContextService appContext)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (appContext == null)
            {
                throw new ArgumentNullException(nameof(appContext));
            }

            this.activity = new Activity
            {
                id = id,
                definition = definition,
            };

            this.appContext = appContext;
        }

        /// <summary>
        /// Creates an xAPI activity from a resource in the corpus.
        /// </summary>
        /// <param name="resource">A corpus item.</param>
        /// <returns>An xAPI activity for the item.</returns>
        public static Activity FromResource(IRemoteResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return Create()
                .SetRelativeId(resource is IExperienceAPIActivity activity ? activity.Id : resource.Url)
                .SetName(resource.Name)
                .SetDescription(resource is IItem item ? item.Description : null)
                .SetActivityTypeBasedOn(resource)
                .GetActivity();
        }

        /// <summary>
        /// Creates an xAPI activity from a prompt in the corpus.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <returns>The xAPI activity.</returns>
        public static Activity FromPrompt(IPrompt prompt)
        {
            if (prompt == null)
            {
                throw new ArgumentNullException(nameof(prompt));
            }

            return Create()
                .SetRelativeId(prompt.Url)
                .SetName(prompt.Question)
                .SetActivityTypeBasedOn(prompt)
                .GetActivity();
        }

        /// <summary>
        /// Creates an xAPI activity from a quiz question.
        /// </summary>
        /// <param name="quiz">The quiz question.</param>
        /// <returns>An xAPI activity for the question.</returns>
        public static Activity QuizAsAssessment(IQuiz quiz)
        {
            if (quiz == null)
            {
                throw new ArgumentNullException(nameof(quiz));
            }

            var questionId = new Uri($"{quiz.Url.AbsolutePath}#assessment", UriKind.Relative);

            return Create()
                .SetRelativeId(questionId)
                .SetName(quiz.Name)
                .SetActivityType(Profiles.Assessment.ActivityTypes.Assessment)
                .GetActivity();
        }

        /// <summary>
        /// Creates an xAPI activity from a file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>An activity for the specified file.</returns>
        public static Activity FromFile(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return Create()
                .SetId(file.Url)
                .SetName(file.Name)
                .SetActivityType(Profiles.Perls.ActivityTypes.File)
                .GetActivity();
        }

        /// <summary>
        /// Creates an xAPI activity from a notification.
        /// </summary>
        /// <param name="action">The notification action.</param>
        /// <returns>An activity for the specified notification.</returns>
        public static Activity FromNotification(string action)
        {
            return Create()
                .SetName(action)
                .SetActivityType(Profiles.Perls.ActivityTypes.Alert)
                .SetRelativeId(new Uri("push_notification", UriKind.Relative))
                .GetActivity();
        }

        /// <summary>
        /// Creates the group for notifications.
        /// </summary>
        /// <param name="action">The notification type.</param>
        /// <returns>A group activity for the given notification.</returns>
        public static Activity NotificationGroup(string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentNullException(action);
            }

            return Create()
                .SetActivityType(Profiles.Perls.ActivityTypes.Tag)
                .SetId(new Uri(Profiles.Perls.ActivityTypes.Tags + "/" + action))
                .GetActivity();
        }

        /// <summary>
        /// Creates an xAPI activity from a media episode.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <returns>An activity from the specified episode.</returns>
        public static Activity FromEpisode(IEpisode episode)
        {
            if (episode == null)
            {
                throw new ArgumentNullException(nameof(episode));
            }

            return Create()
                .SetName(episode.Name)
                .SetDescription(episode.Description)
                .SetActivityTypeBasedOn(episode)
                .SetRelativeId(episode.Url)
                .GetActivity();
        }

        /// <summary>
        /// Creates an xAPI activity from a certificate.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <returns>An activity for the specified certificate.</returns>
        public static Activity FromCertificate(ICertificate certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            return Create()
                .SetRelativeId(new Uri($"/achievement/{certificate.UUID}/certificate"))
                .SetName($"Certificate for completing {certificate.CertificateName}")
                .SetActivityType(Profiles.Perls.ActivityTypes.Certificate)
                .GetActivity();
        }

        /// <summary>
        /// Creates an xAPI activity from a badge.
        /// </summary>
        /// <param name="badge">The badge.</param>
        /// <returns>An activity for the specific badge.</returns>
        public static Activity FromBadge(IBadge badge)
        {
            if (badge == null)
            {
                throw new ArgumentNullException(nameof(badge));
            }

            return Create()
                .SetRelativeId(new Uri($"/group/{badge.UUID}/badge"))
                .SetName($"Badge for completing {badge.Label}")
                .SetActivityType(Profiles.Perls.ActivityTypes.Badge)
                .GetActivity();
        }

        /// <summary>
        /// Creates an xAPI activity for the current application.
        /// </summary>
        /// <returns>An activity for the current application.</returns>
        public static Activity FromApplication()
        {
            var activity = Create();

            return activity
                .SetId(activity.appContext.Server)
                .SetName(activity.appContext.Name)
                .SetActivityType(ActivityTypes.Application)
                .GetActivity();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ActivityBuilder"/> class.
        /// </summary>
        /// <returns>An ActivityBuilder.</returns>
        public static ActivityBuilder Create()
        {
            return new ActivityBuilder();
        }

        /// <summary>
        /// Retrieves the created xAPI activity.
        /// </summary>
        /// <returns>The xAPI activity.</returns>
        public Activity GetActivity()
        {
            return activity;
        }

        /// <summary>
        /// Set the ID for the built activity using a relative URI.
        /// </summary>
        /// <param name="id">The new activity ID.</param>
        /// <returns>This builder instance.</returns>
        public ActivityBuilder SetRelativeId(Uri id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var uri = new Uri(appContext.Server, id.IsAbsoluteUri ? id.AbsolutePath : id.OriginalString);
            return SetId(uri);
        }

        /// <summary>
        /// Set the ID for the built activity.
        /// </summary>
        /// <param name="id">The new activity ID.</param>
        /// <returns>This builder instance.</returns>
        public ActivityBuilder SetId(Uri id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return new ActivityBuilder(id, activity.definition, appContext);
        }

        /// <summary>
        /// Set the name for the built activity using the current culture for the language map.
        /// </summary>
        /// <param name="name">The activity name.</param>
        /// <returns>This builder instance.</returns>
        public ActivityBuilder SetName(string name)
        {
            PrepareDefinition().name = GetLanguageMap(CultureInfo.CurrentCulture, RemoveHTML(name));
            return this;
        }

        /// <summary>
        /// Set the description for the built activity using the current culture for the language map.
        /// </summary>
        /// <param name="description">The activity description.</param>
        /// <returns>This builder instance.</returns>
        public ActivityBuilder SetDescription(string description)
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                PrepareDefinition().description = GetLanguageMap(CultureInfo.CurrentCulture, RemoveHTML(description));
            }

            return this;
        }

        /// <summary>
        /// Derive the activity type for the built activity using a remote resource.
        /// </summary>
        /// <param name="resource">A remote resource whose type defines the activity type.</param>
        /// <returns>This builder instance.</returns>
        public ActivityBuilder SetActivityTypeBasedOn(IRemoteResource resource)
        {
            PrepareDefinition().type = resource switch
            {
                ICourse _ => Profiles.Cmi5.ActivityTypes.Course,
                IFlashcard _ => Profiles.Flashcard.ActivityTypes.Flashcard,
                IQuiz _ or IPrompt _ => Profiles.Assessment.ActivityTypes.Question,
                ITip _ => ActivityTypes.Tip,
                ITopic _ => ActivityTypes.Category,
                ITag _ => ActivityTypes.Tag,
                ITest _ => Profiles.Assessment.ActivityTypes.Assessment,
                IEpisode _ => Profiles.Audio.ActivityTypes.Audio,
                _ => ActivityTypes.Article,
            };

            return this;
        }

        /// <summary>
        /// Set the activity type for the built activity.
        /// </summary>
        /// <param name="activityType">The activity type for the activity.</param>
        /// <returns>This builder instance.</returns>
        public ActivityBuilder SetActivityType(Uri activityType)
        {
            PrepareDefinition().type = activityType;
            return this;
        }

        static string RemoveHTML(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.RemoveHTMLFromString();
        }

        static LanguageMap GetLanguageMap(CultureInfo cultureInfo, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var dictionary = new Dictionary<string, string>
            {
                [cultureInfo.IetfLanguageTag] = value,
            };

            return new LanguageMap(dictionary);
        }

        ActivityDefinition PrepareDefinition()
        {
            return activity.definition;
        }
    }
}
