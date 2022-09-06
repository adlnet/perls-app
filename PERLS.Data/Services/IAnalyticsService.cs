namespace PERLS.Data.Services
{
    /// <summary>
    /// An interface for analytics.
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Set the user ID to report with analytics.
        /// </summary>
        /// <param name="userId">The user ID to report.</param>
        void SetUserId(string userId);
    }
}
