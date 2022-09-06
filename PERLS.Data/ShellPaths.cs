using System;

namespace PERLS.Data
{
    /// <summary>
    /// Holds static Xamarin Shell URL paths.
    /// </summary>
    public static class ShellPaths
    {
        /// <summary>
        /// Gets a static path to the goals page.
        /// </summary>
        /// <value>Static path to the goals page.</value>
        public static string GoalsPath => "//me/stats";

        /// <summary>
        /// Gets a static path to the podcast page.
        /// </summary>
        /// <value>Static path to the podcast page.</value>
        public static string PodcastPath => "//podcasts";

        /// <summary>
        /// Gets a static path to the badge page.
        /// </summary>
        /// <value>Static path to the podcast page.</value>
        public static string BadgePath => "//me/badges";

        /// <summary>
        /// Gets a static path to the certificate page.
        /// </summary>
        /// <value>Static path to the podcast page.</value>
        public static string CertificatePath => "//me/certificates";
    }
}
