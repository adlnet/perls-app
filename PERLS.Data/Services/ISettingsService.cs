using System;

namespace PERLS.Data
{
    /// <summary>
    /// A service for opening the app's settings within the device Settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets a value indicating whether the settings app can be opened.
        /// </summary>
        /// <returns>A value indicating whether the settings app can be opened.</returns>
        bool CanOpenSettings();

        /// <summary>
        /// Opens the app's settings.
        /// </summary>
        void OpenAppSettings();
    }
}
