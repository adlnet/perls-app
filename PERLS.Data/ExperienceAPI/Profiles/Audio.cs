using System;
using TinCan;

namespace PERLS.Data.ExperienceAPI.Profiles
{
    /// <summary>
    /// Definitions for the audio xAPI profile.
    /// </summary>
    /// <see href="https://w3id.org/xapi/audio"/>
    internal static class Audio
    {
        internal static class ActivityTypes
        {
            public static readonly Uri Audio = new Uri("https://w3id.org/xapi/audio/activity-type/audio");
        }

        internal static class Verbs
        {
            public static readonly Verb Initialized = new Verb(new Uri("http://adlnet.gov/expapi/verbs/initialized"), "en-US", "initialized");
            public static readonly Verb Completed = Verb.Completed;
            public static readonly Verb Played = new Verb(new Uri("https://w3id.org/xapi/audio/verbs/played"), "en-US", "played");
            public static readonly Verb Paused = new Verb(new Uri("https://w3id.org/xapi/audio/verbs/paused"), "en-US", "paused");
            public static readonly Verb Seeked = new Verb(new Uri("https://w3id.org/xapi/audio/verbs/seeked"), "en-US", "seeked");
            public static readonly Verb Terminated = Verb.Terminated;
        }

        internal static class Extensions
        {
            public static readonly Uri TimeFrom = new Uri("https://w3id.org/xapi/video/extensions/time-from");
            public static readonly Uri TimeTo = new Uri("https://w3id.org/xapi/video/extensions/time-to");
            public static readonly Uri Length = new Uri("https://w3id.org/xapi/video/extensions/length");
            public static readonly Uri Speed = new Uri("https://w3id.org/xapi/video/extensions/speed");
            public static readonly Uri Progress = new Uri("https://w3id.org/xapi/video/extensions/progress");
            public static readonly Uri Time = new Uri("https://w3id.org/xapi/video/extensions/time");
        }
    }
}
