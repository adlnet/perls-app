using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ViewModels;
using PERLS.Resources;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// The theme provider.
    /// </summary>
    public class DynamicThemingService : IThemingService
    {
        static readonly ResourceDictionary DefaultColors = new Colors();
        static readonly IEnumerable<string> ThemableColors = DefaultColors.Keys;

        /// <inheritdoc/>
        public void ApplyTheme(Application application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            foreach (var resourceName in ThemableColors)
            {
                if (Preferences.ContainsKey(resourceName))
                {
                    application.Resources[resourceName] = Color.FromHex(Preferences.Get(resourceName, null));
                }
            }

            if (Preferences.ContainsKey(nameof(IAppearance.Logo)))
            {
                LogoNavBarViewModel.LogoLocation = Preferences.Get(nameof(IAppearance.Logo), null);
            }
        }

        /// <inheritdoc/>
        public void ResetTheme(Application application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            foreach (var resource in DefaultColors)
            {
                if (resource.Value is Color color)
                {
                    if (Preferences.ContainsKey(resource.Key))
                    {
                        Preferences.Remove(resource.Key);
                    }

                    application.Resources[resource.Key] = color;
                }
            }

            Preferences.Remove(nameof(IAppearance.Logo));
            LogoNavBarViewModel.LogoLocation = null;
        }

        /// <inheritdoc/>
        public async Task<IAppearance> UpdateTheme()
        {
            var corpusProvider = DependencyService.Get<ICorpusProvider>();
            var appearance = await corpusProvider.GetAppearance().ConfigureAwait(false);
            var theme = appearance.Theme;

            if (theme == null || theme.Palette == null)
            {
                return appearance;
            }

            foreach (var entry in theme.Palette)
            {
                if (theme.ColorNameForKeys.ContainsKey(entry.Key))
                {
                    Preferences.Set(theme.ColorNameForKeys[entry.Key], entry.Value);
                }
            }

            if (!string.IsNullOrEmpty(appearance.Logo))
            {
                Preferences.Set(nameof(IAppearance.Logo), appearance.Logo);
            }

            return appearance;
        }
    }
}
