using System;

namespace PERLS.Data.ExperienceAPI.Profiles
{
    /// <summary>
    /// Definitions for the flashcard xAPI profile.
    /// </summary>
    /// <see href="https://w3id.org/xapi/flashcards"/>
    internal static class Flashcard
    {
        internal static class ActivityTypes
        {
            public static readonly Uri Flashcard = new Uri("https://w3id.org/xapi/flashcards/activity-types/flashcard");
        }
    }
}
