using System;

namespace PERLS
{
    /// <summary>
    /// The app configuration settings.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Gets the base URL used for communicating with the API.
        /// </summary>
        /// <remarks>
        /// This should be configurable at runtime on non-production builds.
        /// </remarks>
        /// <value>The base URL.</value>
        public static Uri DefaultServer => new Uri("https://perls.usalearning.net");

        /// <summary>
        /// Gets the App Center identifier.
        /// </summary>
        /// <value>The App Center identifier.</value>
        public static Guid AppCenterId => new Guid("00000000-0000-0000-0000-000000000000");

        /// <summary>
        /// Gets the build configuration that was set at build time.
        /// </summary>
        /// <value>The build configuration that was set at build time.</value>
        public const BuildConfiguration Configuration = BuildConfiguration.Release;

        /// <summary>
        /// Gets the build flavor that was set at build time.
        /// </summary>
        /// <value>The build flavor that was set at build time.</value>
        public const BuildFlavor Flavor = BuildFlavor.Release;

        /// <summary>
        /// Gets the current package identifier.
        /// </summary>
        /// <value>The package identifier.</value>
        public const string PackageIdentifier = "net.usalearning.perls";

        /// <summary>
        /// Indicates whether or not team entry should be enabled for this build.
        /// </summary>
        public const bool TeamEntry = true;

        /// <summary>
        /// Indicates whether or not offline access should be enabled for this build.
        /// </summary>
        public const bool OfflineAccess = true;

        /// <summary>
        /// Indicates whether the build prefers to use local accounts as opposed to federated accounts.
        /// </summary>
        public const bool PrefersLocalAuthentication = false;

        /// <summary>
        /// Indicates whether or not podcasts should be enabled for this build.
        /// </summary>
        public const bool Podcasts = true;

        /// <summary>
        /// Indicates whether to show terms of use instead of the Float legal info page.
        /// </summary>
        public const bool TermsOfUse = true;

        /// <summary>
        /// Indicates whether or not feedback access should be enabled for this build.
        /// </summary>
        public const bool FeedbackAccess = true;

        /// <summary>
        /// Indicates whether or not stats access should be enabled for this build.
        /// </summary>
        public const bool StatsAccess = true;

        /// <summary>
        /// Indicates whether or not interests access should be enabled for this build.
        /// </summary>
        public const bool InterestsAccess = true;

        /// <summary>
        /// Indicates whether or not notes access should be enabled for this build.
        /// </summary>
        public const bool NotesAccess = true;

        /// <summary>
        /// Indicates whether or not groups setting should be enabled for this build.
        /// </summary>
        public const bool GroupsSetting = true;

        /// <summary>
        /// Indicates whether or not the account setting should be enabled for this build.
        /// </summary>
        public const bool AccountSetting = true;

        /// <summary>
        /// Indicates whether or not the support setting should be enabled for this build.
        /// </summary>
        public const bool SupportSetting = false;

        /// <summary>
        /// Indicates whether or not the tag following should be enabled for this build.
        /// </summary>
        public const bool TagFollowing = true;

        /// <summary>
        /// Indicates whether or not the privacy policy should be enabled for this build.
        /// </summary>
        public const bool PrivacyPolicySetting = true;

        /// <summary>
        /// Indicates whether or not the native Unity framework should be enabled for this build.
        /// </summary>
        public const bool EnableUnityFramework = false;

        /// <summary>
        /// The full semantic version of the built application.
        /// </summary>
        public const string FullSemVersion = "0.0.0-feat.1+1";

        /// <summary>
        /// The name of this application.
        /// </summary>
        public const string AppName = "PERLS";

        /// <summary>
        /// Gets the ClientID.
        /// </summary>
        /// <value>The client ID.</value>
        public const string ClientId = "";

        /// <summary>
        /// Gets the Client Secret.
        /// </summary>
        /// <value>The client secret.</value>
        public const string ClientSecret = "";

        /// <summary>
        /// Gets the URL for sending support requests.
        /// </summary>
        /// <value>The URL for support requests.</value>
        public static Uri SupportPath => new Uri("https://www.example.com/support");

        /// <summary>
        /// Gets the legal info path.
        /// </summary>
        /// <value>The legal info path.</value>
        public const string LegalInfoPath = "https://www.example.com/privacy-policy";

        /// <summary>
        /// Gets a static string to be included in the user agent string.
        /// </summary>
        public const string UserAgent = "PERLS";

        /// <summary>
        /// Gets the default server to be pre-filled for debug builds.
        /// </summary>
        public const string DebugDefaultServer = "";

        /// <summary>
        /// Gets the OAuth redirect URI.
        /// </summary>
        public const string OAuthRedirectUri = "https://example.com/auth";

        /// <summary>
        /// Gets the allowed host name.
        /// </summary>
        public const string AllowedHost = "usalearning.net";

        /// <summary>
        /// Gets the default reachability URI.
        /// </summary>
        public static Uri DefaultReachabilityUri => new Uri("https://perls.usalearning.net");

        /// <summary>
        /// Gets the expected text for messages sent from Unity.
        /// </summary>
        public const string UnityMessageLabel = "ExampleMessage";
    }
}
