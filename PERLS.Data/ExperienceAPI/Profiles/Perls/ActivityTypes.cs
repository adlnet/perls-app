using System;

namespace PERLS.Data.ExperienceAPI.Profiles.Perls
{
    /// <summary>
    /// Definitions for xAPI usage that is (currently) specific to PERLS.
    /// </summary>
    public static class ActivityTypes
    {
        /// <summary>
        /// Represents objects such as news articles, knowledge base entries, or other similar construct.
        /// </summary>
        public static readonly Uri Article = new Uri("http://activitystrea.ms/schema/1.0/article");

        /// <summary>
        /// Activity generally used in the "grouping" Context Activities lists to mark a statement as being related to a particular subject area.
        /// </summary>
        public static readonly Uri Category = new Uri("http://id.tincanapi.com/activitytype/category");

        /// <summary>
        /// Activity generally used in the "other" or "grouping" Context Activities lists to mark a statement as being related to a particular subject area.
        /// </summary>
        public static readonly Uri Tag = new Uri("http://id.tincanapi.com/activitytype/tag");

        /// <summary>
        /// Activity generally used to mark the group of "tag" statements.
        /// </summary>
        public static readonly Uri Tags = new Uri("http://id.tincanapi.com/activitytype/tags");

        /// <summary>
        /// A resource is a generic item that the actor may use for something.
        /// </summary>
        public static readonly Uri Tip = new Uri("http://id.tincanapi.com/activitytype/resource");

        /// <summary>
        /// A goal activity type.
        /// </summary>
        public static readonly Uri Goal = new Uri("http://id.tincanapi.com/activitytype/goal");

        /// <summary>
        /// An alert activity type.
        /// </summary>
        public static readonly Uri Alert = new Uri("http://activitystrea.ms/schema/1.0/alert");

        /// <summary>
        /// A certificate activity type.
        /// </summary>
        public static readonly Uri Certificate = new Uri("https://www.opigno.org/en/tincan_registry/activity_type/certificate");

        /// <summary>
        /// Represents any form of document or file.
        /// </summary>
        public static readonly Uri File = new Uri("http://activitystrea.ms/schema/1.0/file");

        /// <summary>
        /// Represents a badge or award granted to an object (typically a person object).
        /// </summary>
        public static readonly Uri Badge = new Uri("http://activitystrea.ms/schema/1.0/badge");

        /// <summary>
        /// Represents a grouping of objects in which member objects can join or leave.
        /// </summary>
        public static readonly Uri Group = new Uri("http://activitystrea.ms/schema/1.0/group");

        /// <summary>
        /// Represents the application as a whole.
        /// </summary>
        public static readonly Uri Application = new Uri("http://activitystrea.ms/schema/1.0/application");
    }
}
