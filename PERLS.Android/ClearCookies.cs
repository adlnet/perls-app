using Android.Webkit;
using PERLS.Data.Definition.Services;
using PERLS.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(ClearCookies))]

namespace PERLS.Droid
{
    /// <summary>
    /// Provides acces to clear cookies.
    /// </summary>
    public class ClearCookies : IClearCookiesService
    {
        /// <summary>
        /// Provides acces to clear cookies.
        /// </summary>
        public void ClearAllCookies()
        {
            var cookieManager = CookieManager.Instance;
            cookieManager.RemoveAllCookie();
        }
    }
}
