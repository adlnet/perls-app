namespace PERLS
{
    /// <summary>
    /// The app configuration settings.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Gets path for starting a token session with the cms.
        /// </summary>
        /// <value>The token session path.</value>
        public static string TokenSessionPath => "/start-session/";

        /// <summary>
        /// Gets string to add the destination query.
        /// </summary>
        /// <value>The destination query.</value>
        public static string DestinationQuery => "destination=";

        /// <summary>
        /// Gets the Make a Suggestion destination path.
        /// </summary>
        /// <value>The make a suggestion destination path.</value>
        public static string SuggestionsPath => "/suggestions";

        /// <summary>
        /// Gets the comments destination path.
        /// </summary>
        /// <value>
        /// The comments destination path.
        /// </value>
        public static string DiscussionPath => "/discussion";

        /// <summary>
        /// Gets the prompts destination path.
        /// </summary>
        /// <value>
        /// The prompts destination path.
        /// </value>
        public static string PromptsPath => "/prompts";

        /// <summary>
        /// Gets the history destination path.
        /// </summary>
        /// <value>
        /// The history destination path.
        /// </value>
        public static string HistoryPath => "/history";

        /// <summary>
        /// Gets the bookmarks destination path.
        /// </summary>
        /// <value>
        /// The bookmarks destination path.
        /// </value>
        public static string BookmarksPath => "/bookmarks";

        /// <summary>
        /// Gets the groups destination path.
        /// </summary>
        /// <value>
        /// The groups destination path.
        /// </value>
        public static string GroupsPath => "/groups";

        /// <summary>
        /// Gets the following destination path.
        /// </summary>
        /// <value>
        /// The following destination path.
        /// </value>
        public static string FollowingPath => "/following";

        /// <summary>
        /// Gets string to add the terms of use path.
        /// </summary>
        /// <value>String to add the terms of use path.</value>
        public static string TermsOfUsePath => "terms-of-use";

        /// <summary>
        /// Gets the Vidyo room starting path.
        /// </summary>
        /// <value>
        /// The Vidyo room starting path.
        /// </value>
        public static string VidyoRoomPath => "/vidyo-room";

        /// <summary>
        /// Gets string to edit a goal.
        /// </summary>
        /// <value>
        /// String to edit a goal.
        /// </value>
        public static string EditGoalPath => "/goal";

        /// <summary>
        /// Gets a string to specify the reminder alerts messaging center channel.
        /// </summary>
        /// <value>The reminder alerts messaging center channel.</value>
        public static string DisplayReminderAlert => "DisplayReminderAlert";
    }
}
