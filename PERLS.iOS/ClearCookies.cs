using Foundation;
using PERLS.Data.Definition.Services;
using PERLS.iOS;
using PERLS.iOS.Renderers;
using WebKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ClearCookies))]

namespace PERLS.iOS
{
    /// <summary>
    /// Provides access to clear cookies.
    /// </summary>
    public class ClearCookies : IClearCookiesService
    {
        /// <summary>
        /// Provides acces to clear cookies.
        /// </summary>
        /// <remarks>
        /// In addition to cookies, it attempts to clear all website data (in case private information was stored).
        /// </remarks>
        public void ClearAllCookies()
        {
            ClearDataStore(WebViewRenderer.GlobalDataStore);
            NSHttpCookieStorage.SharedStorage.RemoveCookiesSinceDate(NSDate.DistantPast);
        }

        void ClearDataStore(WKWebsiteDataStore store)
        {
            store.RemoveDataOfTypes(WKWebsiteDataStore.AllWebsiteDataTypes, NSDate.DistantPast, () => { });
            store.HttpCookieStore.GetAllCookies(cookies =>
            {
                foreach (var cookie in cookies)
                {
                    store.HttpCookieStore.DeleteCookie(cookie, null);
                }
            });
        }
    }
}
