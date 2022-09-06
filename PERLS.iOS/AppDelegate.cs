using System;
using System.Collections.Generic;
using FFImageLoading.Forms.Platform;
using Firebase.CloudMessaging;
using Float.Core.Analytics;
using Foundation;
using MediaManager;
using Newtonsoft.Json;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Services;
using PERLS.iOS.Services;
using PERLS.Pages;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace PERLS.iOS
{
    /// <summary>
    /// The app delegate.
    /// </summary>
    [Register(nameof(AppDelegate))]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        const string FirebasePlist = "google-services.plist";

        App application;

        /// <inheritdoc />
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            if (uiApplication == null)
            {
                throw new ArgumentNullException(nameof(uiApplication));
            }

            using (var firebaseOptions = new Firebase.Core.Options(FirebasePlist))
            {
                Firebase.Core.App.Configure(firebaseOptions);
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            Messaging.SharedInstance.Delegate = this;

            var attributes = new UITextAttributes
            {
                TextColor = new UIColor((System.nfloat)0.0, (System.nfloat)0.0),
            };

            CrossMediaManager.Current.Init();

            Rg.Plugins.Popup.Popup.Init();
            CachedImageRenderer.Init();
            Forms.Init();

            var analytics = new Analytics.AppCenterAnalytics();
            DependencyService.RegisterSingleton(analytics);
            DependencyService.RegisterSingleton<IAnalyticsService>(analytics);

            DependencyService.Register<FirebaseMessagingService>();
            DependencyService.Register<NotificationAccessService>();
            DependencyService.Get<AnalyticsService>().TrackPageView(nameof(AppDelegate));
            DependencyService.Register<AppleFileProcessor>();
            DependencyService.Register<BrowserService>();
            DependencyService.Register<ShareService>();
            DependencyService.Register<CalendarService>();

#pragma warning disable CS0162 // Unreachable code detected
            if (Constants.EnableUnityFramework)
            {
                DependencyService.Register<UnityService>();
            }
#pragma warning restore CS0162 // Unreachable code detected

            application = new App();

            this.LoadApplication(application);
            UINavigationBar.Appearance.Translucent = false;
            UIBarButtonItem.Appearance.TintColor = UIColor.Gray;
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
            {
                Font = UIFont.FromName("Open Sans", 18f),
            });
            var attr = new UITextAttributes();
            attr.TextColor = ((Color)Xamarin.Forms.Application.Current.Resources[nameof(ITheme.PrimaryTextColor)]).ToUIColor();
            UINavigationBar.Appearance.SetTitleTextAttributes(attr);
            UIBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes
                {
                    Font = UIFont.FromName("Open Sans", 18f),
                },
                UIControlState.Normal);

            UITabBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes
                {
                    Font = UIFont.FromName("Open Sans", 11f),
                },
                UIControlState.Normal);

            var result = base.FinishedLaunching(uiApplication, launchOptions);
            uiApplication.KeyWindow.TintColor = ((Color)Xamarin.Forms.Application.Current.Resources[nameof(ITheme.SecondaryColor)]).ToUIColor();

#if DEBUG
            Xamarin.Calabash.Start();
#endif
            return result;
        }

        /// <summary>
        /// Handle incoming notification messages while app is in the foreground.
        /// </summary>
        /// <param name="center">Notification center object.</param>
        /// <param name="notification">The notification object.</param>
        /// <param name="completionHandler">The completion handler.</param>
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
#pragma warning disable CA1801 // Review unused parameters
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
#pragma warning restore CA1801 // Review unused parameters
        {
            completionHandler?.Invoke(UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Alert);
        }

        /// <summary>
        /// Handle notification messages after display notification is tapped by the user.
        /// </summary>
        /// <param name="center">Notification center object.</param>
        /// <param name="response">The response object.</param>
        /// <param name="completionHandler">The completion handler.</param>
        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
#pragma warning disable CA1801 // Review unused parameters
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
#pragma warning restore CA1801 // Review unused parameters
        {
            if (response?.Notification?.Request?.Content?.UserInfo["action"]?.ToString() is string action)
            {
                var notificationNavigation = DependencyService.Get<INotificationNavigation>();
                Dictionary<string, string> itemDict = null;
                Dictionary<string, string> relatedItemDict = null;

                if (response?.Notification?.Request?.Content?.UserInfo["item"]?.ToString() is string item)
                {
                    try
                    {
                        itemDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(item);
                    }
                    catch (JsonSerializationException e)
                    {
                        DependencyService.Get<AnalyticsService>().TrackException(e);
                    }

                    if (response?.Notification?.Request?.Content?.UserInfo["related_items"]?.ToString() is string relatedItems)
                    {
                        try
                        {
                            relatedItemDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(relatedItems);
                        }
                        catch (JsonSerializationException e)
                        {
                            DependencyService.Get<AnalyticsService>().TrackException(e);
                        }
                    }
                }

                notificationNavigation.NavigateByAction(action, itemDict, relatedItemDict);
            }

            completionHandler?.Invoke();
        }

        /// <inheritdoc/>
        public override void ReceiveMemoryWarning(UIApplication application)
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        /// <inheritdoc/>
        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            if (Shell.Current?.CurrentPage is BasePage page && page.AllowLandscape)
            {
                return UIInterfaceOrientationMask.All;
            }
            else if (App.Current?.MainPage is NavigationPage navigationPage && navigationPage.RootPage is BasePage basePage && basePage.AllowLandscape)
            {
                return UIInterfaceOrientationMask.All;
            }
            else
            {
                return UIInterfaceOrientationMask.Portrait;
            }
        }
    }
}
