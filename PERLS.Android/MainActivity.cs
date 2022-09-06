using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using FFImageLoading.Svg.Forms;
using Float.Core.Analytics;
using MediaManager;
using Newtonsoft.Json;
using PERLS.Data;
using PERLS.Data.Definition.Services;
using PERLS.Data.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Android.Net;
using Xamarin.Forms;

namespace PERLS.Droid
{
    /// <summary>
    /// The main android activity.
    /// </summary>
    [Activity(
        Label = Constants.AppName,
        Icon = "@mipmap/ic_launcher",
        RoundIcon ="@mipmap/ic_launcher_round",
        Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Unspecified,
        LaunchMode = LaunchMode.SingleInstance,
        MainLauncher = true,
        Name = Constants.PackageIdentifier + ".MainActivity")]
    [IntentFilter(new[] { "HandleNotification" }, Categories = new[] { Intent.CategoryDefault })]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <summary>
        /// Request permissions for xamarin essentials.
        /// </summary>
        /// <param name="requestCode">The request code.</param>
        /// <param name="permissions">The permissions.</param>
        /// <param name="grantResults">The Grant Results.</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// Ons the back pressed.
        /// </summary>
        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                if (PopupNavigation.Instance.PopupStack.Count > 0)
                {
                    PopupNavigation.Instance.PopAsync();
                }
            }
        }

        /// <summary>
        /// Ons the memory trimmed.
        /// </summary>
        /// <param name="level">The memory trim level.</param>
        public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.OnTrimMemory(level);
        }

        /// <inheritdoc/>
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent?.GetStringExtra("action") is string action)
            {
                var notificationNavigation = DependencyService.Get<INotificationNavigation>();
                Dictionary<string, string> itemDict = null;
                Dictionary<string, string> relatedItemsDict = null;

                if (intent?.GetStringExtra("item") is string item)
                {
                    try
                    {
                        itemDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(item);
                    }
                    catch (JsonSerializationException e)
                    {
                        DependencyService.Get<AnalyticsService>().TrackException(e);
                    }

                    if (intent?.GetStringExtra("related_items") is string relatedItems)
                    {
                        try
                        {
                            relatedItemsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(relatedItems);
                        }
                        catch (JsonSerializationException e)
                        {
                            DependencyService.Get<AnalyticsService>().TrackException(e);
                        }
                    }
                }

                notificationNavigation.NavigateByAction(action, itemDict, relatedItemsDict);
            }
        }

        /// <summary>
        /// On Create.
        /// </summary>
        /// <param name="savedInstanceState">The Saved Instance State.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            var handler = new AndroidClientHandler();
            handler.Dispose();

            base.OnCreate(savedInstanceState);

            CrossMediaManager.Current.Init(this);
            UserDialogs.Init(this);
            Rg.Plugins.Popup.Popup.Init(this);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            var ignore = typeof(SvgCachedImage);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            var analytics = new Analytics.AppCenterAnalytics();
            DependencyService.RegisterSingleton(analytics);
            DependencyService.RegisterSingleton<IAnalyticsService>(analytics);
            DependencyService.Register<FirebaseMessagingService>();
            DependencyService.Register<NotificationAccessService>();
            DependencyService.Get<AnalyticsService>().TrackPageView(nameof(MainActivity));
            DependencyService.Register<AndroidFileProcessor>();
            DependencyService.Register<BrowserService>();
            DependencyService.Register<CalendarService>();

#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.Configuration == BuildConfiguration.Debug)
            {
                Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
            }
#pragma warning restore CS0162 // Unreachable code detected

            if (IsPlayServicesAvailable())
            {
                CreateFirebaseNotificationChannel();
            }

            MessagingCenter.Subscribe<FloatFirebaseMessagingService, string>(
                this,
                Strings.FirebaseTokenMessage,
                (sender, token) =>
                {
                    DependencyService.Get<FirebaseMessagingService>().UpdatePushToken(token).ConfigureAwait(false);
                });

            // this prevents the splash bg appearing in modals
            SetTheme(Resource.Style.BlankTheme);

            // this prevents the splash bg appearing everywhere else
            Window.SetBackgroundDrawable(null);

            LoadApplication(new App());

            OnNewIntent(Intent);
        }

        private bool IsPlayServicesAvailable()
        {
            return GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this) == ConnectionResult.Success;
        }

        void CreateFirebaseNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(FloatFirebaseMessagingService.FirebaseChannelId, Strings.NotificationChannel, NotificationImportance.High);
            channel.Description = "Firebase Cloud Messages appear in this channel";
            channel.LockscreenVisibility = NotificationVisibility.Public;
            channel.Name = Constants.AppName;

            var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
            channel.Dispose();
        }
    }
}
