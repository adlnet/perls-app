using System;
using TinCan;

namespace PERLS.Data.ExperienceAPI.Profiles.Perls
{
    /// <summary>
    /// A collection of verbs for the PERLS profile.
    /// </summary>
    public static class Verbs
    {
        /// <summary>
        /// Indicates that the actor has viewed the object.
        /// </summary>
        public static readonly Verb Viewed = GetVerb("http://id.tincanapi.com/verb/viewed", "viewed");

        /// <summary>
        /// Indicates that the actor has experienced the object in some manner. Note that, depending on the specific object types
        /// used for both the actor and object, the meaning of this verb can overlap that of the "consume" and "play" verbs. For
        /// instance, a person might "experience" a movie; or "play" the movie; or "consume" the movie. The "experience" verb can be
        /// considered a more generic form of other more specific verbs as "consume", "play", "watch", "listen", and "read".
        /// </summary>
        public static readonly Verb Experienced = GetVerb("http://adlnet.gov/expapi/verbs/experienced", "experienced");

        /// <summary>
        /// Indicates that the actor has interacted with the object. For instance, when one person interacts with another.
        /// </summary>
        public static readonly Verb Interacted = GetVerb("http://adlnet.gov/expapi/verbs/interacted", "interacted");

        /// <summary>
        /// To indicate an actor has passed over or omitted an interval, screen, segment, item, or step.
        /// </summary>
        public static readonly Verb Skipped = GetVerb("http://id.tincanapi.com/verb/skipped", "skipped");

        /// <summary>
        /// Indicates that the actor is or has searched for the object. If a target is specified, it indicates the context within
        /// which the search is or has been conducted.
        /// </summary>
        public static readonly Verb Searched = GetVerb("http://activitystrea.ms/schema/1.0/search", "searched");

        /// <summary>
        /// Indicates that the actor selects an object from a collection or set to use it in an activity. The collection would be
        /// the context parent element.
        /// </summary>
        public static readonly Verb Selected = GetVerb("http://id.tincanapi.com/verb/selected", "selected");

        /// <summary>
        /// Indicates that the actor downloaded (rather than accessed or opened) a file or document.
        /// </summary>
        public static readonly Verb Downloaded = GetVerb("http://id.tincanapi.com/verb/downloaded", "downloaded");

        /// <summary>
        /// Indicates that the actor marked the object as an item of special interest.
        /// </summary>
        public static readonly Verb Favorited = GetVerb("http://activitystrea.ms/schema/1.0/save", "saved");

        /// <summary>
        /// Indicates that the actor has removed the object from the collection of favorited items.
        /// </summary>
        public static readonly Verb Unfavorited = GetVerb("http://activitystrea.ms/schema/1.0/unsave", "unsaved");

        /// <summary>
        /// Used to respond to a question.  It could be either the actual answer to a question asked of the actor OR the
        /// correctness of an answer to a question asked of the actor. Must follow a statement with asked or another statement with
        /// a responded (the top statement with responded) must follow the "asking" statement.  The response to the question can be
        /// the actual text (usually) response or the correctness of that response.  For example, Andy responded to quiz question 1
        /// with result 'response=4' and Andy responded to quiz question 1 with result success=true'.  Typically both types of
        /// responded statements would follow a single question/interacton.
        /// </summary>
        public static readonly Verb Responded = GetVerb("http://adlnet.gov/expapi/verbs/responded", "responded");

        /// <summary>
        /// Offered an opinion or written experience of the activity. Can be used with the learner as the actor or a system as an
        /// actor.  Comments can be sent from either party with the idea that the other will read and react to the content.
        /// </summary>
        public static readonly Verb Commented = GetVerb("http://adlnet.gov/expapi/verbs/commented", "commented");

        /// <summary>
        /// Indicates that the actor has defined the object. Note that this is a more specific form of the verb “create”. For
        /// instance, a learner defining a goal.
        /// </summary>
        public static readonly Verb Defined = GetVerb("http://id.tincanapi.com/verb/defined", "defined");

        /// <summary>
        /// "Indicates that the actor has voted up for a specific object. This is analogous to giving a thumbs up.
        /// </summary>
        public static readonly Verb VotedUp = GetVerb("http://id.tincanapi.com/verb/voted-up", "voted-up");

        /// <summary>
        /// Indicates that the actor has voted down for a specific object. This is analogous to giving a thumbs down.
        /// </summary>
        public static readonly Verb VotedDown = GetVerb("http://id.tincanapi.com/verb/voted-down", "voted-down");

        /// <summary>
        /// Starts the process of launching the next piece of learning content.  There is no expectation if this is done by user or
        /// system and no expectation that the learning content is a "SCO".  It is highly recommended that the display is used to
        /// mirror the behavior.  If an activity is launched from another, then launched from may be better.  If the activity is
        /// launched and then the statement is generated, launched or launched into may be more appropriate.
        /// </summary>
        public static readonly Verb Launched = GetVerb("http://adlnet.gov/expapi/verbs/launched", "launched");

        /// <summary>
        /// Indicates that the actor has called out the object to readers. In most cases, the actor did not create the object being
        /// shared, but is instead drawing attention to it.
        /// </summary>
        public static readonly Verb Shared = GetVerb("http://activitystrea.ms/schema/1.0/share", "shared");

        /// <summary>
        /// Indicates that the actor began following the activity of the object. In most cases, the objectType will be a "person",
        /// but it can potentially be of any type that can sensibly generate activity. Processors MAY ignore (silently drop)
        /// successive identical "follow" activities.
        /// </summary>
        public static readonly Verb Followed = GetVerb("http://activitystrea.ms/schema/1.0/follow", "followed");

        /// <summary>
        /// Indicates that the actor has stopped following the object.
        /// </summary>
        public static readonly Verb Unfollowed = GetVerb("http://activitystrea.ms/schema/1.0/stop-following", "stopped following");

        /// <summary>
        /// Indicates the actor responded to a question.
        /// </summary>
        public static readonly Verb Answered = GetVerb("http://adlnet.gov/expapi/verbs/answered", "answered");

        /// <summary>
        /// A special LRS-reserved verb. Used by the LRS to declare that an activity statement is to be voided from record.
        /// </summary>
        public static readonly Verb Voided = GetVerb("http://adlnet.gov/expapi/verbs/voided", "voided");

        static Verb GetVerb(string id, string display, string languageCode = "en-US")
        {
            return new Verb(new Uri(id), languageCode, display);
        }
    }
}
