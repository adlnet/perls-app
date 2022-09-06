using System;
using TinCan;

namespace PERLS.Data.ExperienceAPI.Profiles
{
    /// <summary>
    /// Definitions for the Assessment xAPI profile.
    /// </summary>
    /// <see href="http://id.tincanapi.com/recipe/assessment/general/1"/>
    internal static class Assessment
    {
        internal static class ActivityTypes
        {
            public static readonly Uri Assessment = new Uri("http://adlnet.gov/expapi/activities/assessment");
            public static readonly Uri Question = new Uri("http://adlnet.gov/expapi/activities/question");
        }

        internal static class Verbs
        {
            public static readonly Verb Answered = new Verb(new Uri("http://adlnet.gov/expapi/verbs/answered"), "en-US", "answered");
            public static readonly Verb Asked = new Verb(new Uri("http://adlnet.gov/expapi/verbs/asked"), "en-US", "asked");
            public static readonly Verb Attempted = new Verb(new Uri("http://adlnet.gov/expapi/verbs/attempted"), "en-US", "attempted");
            public static readonly Verb Passed = Cmi5.Verbs.Passed;
            public static readonly Verb Failed = Cmi5.Verbs.Failed;
            public static readonly Verb Completed = Verb.Completed;
        }
    }
}
