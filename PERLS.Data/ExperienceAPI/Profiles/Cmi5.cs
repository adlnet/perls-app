using System;
using TinCan;

namespace PERLS.Data.ExperienceAPI.Profiles
{
    /// <summary>
    /// Definitions for the Cmi5 xAPI profile.
    /// </summary>
    /// <see href="https://w3id.org/xapi/cmi5"/>
    internal static class Cmi5
    {
        internal static class ActivityTypes
        {
            public static readonly Uri Course = new Uri("http://adlnet.gov/expapi/activities/course");
        }

        internal static class Verbs
        {
            public static readonly Verb Launched = Verb.Launched;
            public static readonly Verb Initialized = new Verb(new Uri("http://adlnet.gov/expapi/verbs/initialized"), "en-US", "initialized");
            public static readonly Verb Completed = Verb.Completed;
            public static readonly Verb Passed = new Verb(new Uri("http://adlnet.gov/expapi/verbs/passed"), "en-US", "passed");
            public static readonly Verb Failed = new Verb(new Uri("http://adlnet.gov/expapi/verbs/failed"), "en-US", "failed");
            public static readonly Verb Terminated = Verb.Terminated;
        }
    }
}
