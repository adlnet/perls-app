using System;
using Android.Content;
using PERLS.Data;
using PERLS.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SettingsService))]

namespace PERLS.Droid
{
    /// <summary>
    /// The Android implementation of SettingsService.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        readonly string action = Android.Provider.Settings.ActionApplicationDetailsSettings;

        /// <inheritdoc/>
        public bool CanOpenSettings()
        {
            var context = Android.App.Application.Context;

            try
            {
                using (var intent = BuildIntent(context))
                {
                    return intent.ResolveActivity(context.PackageManager) != null;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void OpenAppSettings()
        {
            var context = Android.App.Application.Context;

            using (var intent = BuildIntent(context))
            {
                try
                {
                    context.StartActivity(intent);
                }
                catch (ActivityNotFoundException e)
                {
                    System.Diagnostics.Debug.WriteLine($"err: {e}");
                }
            }
        }

        Intent BuildIntent(Context context)
        {
            using (var intent = new Intent(action))
            {
                intent.SetData(Android.Net.Uri.Parse($"package:{context.PackageName}"));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                return intent;
            }
        }
    }
}
