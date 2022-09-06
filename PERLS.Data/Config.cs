using System;
using System.Globalization;
using System.Resources;
using PERLS.Data.Definition.Services;
using PERLS.Services;
using Xamarin.Forms;

[assembly: NeutralResourcesLanguage("en")]

namespace PERLS
{
    /// <summary>
    /// The Debug configuration settings.
    /// </summary>
    public static class AppConfig
    {
        const string ServerKey = "Server";
        const string CultureKey = "culture";

        /// <summary>
        /// Gets or Sets Server used for communication with the API.
        /// </summary>
        /// <value>The API communication server.</value>
        public static Uri Server
        {
            get
            {
                if (Application.Current.Properties.TryGetValue(ServerKey, out var obj)
                    && obj is string server
                    && !string.IsNullOrWhiteSpace(server)
                    && Uri.TryCreate(server, UriKind.Absolute, out var serverUri))
                {
                    return serverUri;
                }

                return Constants.DefaultServer;
            }

            set
            {
                if (value == null)
                {
                    Application.Current.Properties.Remove(ServerKey);
                }
                else
                {
                    Application.Current.Properties[ServerKey] = value.AbsoluteUri;
                }

                _ = Application.Current.SavePropertiesAsync();
            }
        }

        /// <summary>
        /// Gets the Culture.
        /// </summary>
        /// <value>
        /// The Culture.
        /// </value>
        public static CultureInfo Culture
        {
            get
            {
                if (Application.Current.Properties.TryGetValue(CultureKey, out var cachedCulture) && cachedCulture is string cachedCultureString)
                {
                    try
                    {
                        return new CultureInfo(cachedCultureString);
                    }
                    catch (CultureNotFoundException)
                    {
                        return CultureInfo.GetCultureInfo("en");
                    }
                }

                return CultureInfo.GetCultureInfo("en");
            }
        }

        /// <summary>
        /// Updates the culture.
        /// </summary>
        /// <param name="cultureString">The two-letter culture name.</param>
        public static void UpdateCulture(string cultureString)
        {
            if (cultureString == null)
            {
                Application.Current.Properties.Remove(CultureKey);
                _ = Application.Current.SavePropertiesAsync();
                return;
            }

            try
            {
                // We want to verify that a culture can be created with this string.
                _ = new CultureInfo(cultureString);
                Application.Current.Properties[CultureKey] = cultureString;
                _ = Application.Current.SavePropertiesAsync();
            }
            catch (CultureNotFoundException)
            {
                // If the culture can't be created we should remove the value.
                Application.Current.Properties.Remove(CultureKey);
                _ = Application.Current.SavePropertiesAsync();
            }
        }

        /// <summary>
        /// Move the server value from secure storage, if one is set.
        /// </summary>
        public static void UpdateStorage()
        {
            var store = DependencyService.Get<SecureStoreService>();

            if (store.Get(ServerKey) is string server)
            {
                Server = new Uri(server);
                store.Delete(ServerKey);
            }
        }
    }
}
