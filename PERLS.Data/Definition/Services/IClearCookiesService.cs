namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// Provides acces to clear cookies.
    /// </summary>
    public interface IClearCookiesService
    {
        /// <summary>
        /// Clear all the cookies in the app.
        /// </summary>
        void ClearAllCookies();
    }
}
