using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Foundation;
using PERLS.Services;
using UIKit;

#if ENABLE_UNITY_FRAMEWORK
using UnityLibrary.iOS;
#endif

using Xamarin.Forms;

namespace PERLS.iOS.Services
{
    /// <summary>
    /// A wrapper for calling the native Unity framework.
    /// </summary>
    public class UnityService : IUnityService
    {
        readonly UIWindow appWindow;
#if ENABLE_UNITY_FRAMEWORK
        UnityFramework unity;
#endif
        Action onComplete;
        Action onQuit;
        bool isPaused;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityService"/> class.
        /// </summary>
        public UnityService()
        {
            appWindow = RetrieveWindow(keyWindow: true);
        }

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

#if ENABLE_UNITY_FRAMEWORK
        UIWindow UnityWindow => unity.AppController().GetWindow();
#else
        UIWindow UnityWindow => null;
#endif

        /// <inheritdoc/>
        public void ShowUnity()
        {
            if (!IsInitialized)
            {
                InitializeUnity();
            }

            if (isPaused)
            {
#if ENABLE_UNITY_FRAMEWORK
                unity.Pause(false);
#endif
                isPaused = false;
            }

            ShowUnityWindowFromBackground();
            HideAppWindowFromForeground();
        }

        /// <inheritdoc/>
        public void SendMessage(string gameObjectName, string functionName, string message)
        {
#if !ENABLE_UNITY_FRAMEWORK
            throw new NotImplementedException();
#else
            unity.SendMessageToGOWithName(gameObjectName, functionName, message);
#endif
        }

        /// <inheritdoc/>
        public void OnCompleted(Action action)
        {
            onComplete = action;
        }

        /// <inheritdoc/>
        public void OnQuit(Action action)
        {
            onQuit = action;
        }

        void InitializeUnity()
        {
#if !ENABLE_UNITY_FRAMEWORK
            throw new NotImplementedException();
#else
            unity = UnityFramework.GetInstance();

            using var sendToNativeString = new NSString(Constants.UnityMessageLabel);

            NSNotificationCenter.DefaultCenter.AddObserver(sendToNativeString, (notification) =>
            {
                if (notification.UserInfo?["data"]?.ToString() is string data)
                {
                    switch (data)
                    {
                        case "quit":
                            // user quit
                            onQuit?.Invoke();
                            break;
                        case "complete":
                            // user completed
                            onComplete?.Invoke();
                            break;
                    }

                    onQuit = null;
                    onComplete = null;
                }

                unity.Pause(true);
                isPaused = true;

                Device.BeginInvokeOnMainThread(() =>
                {
                    ShowAppWindowFromBackground();
                    HideUnityWindowFromForeground();
                });
            });

            LoadFrameworkBundle("UnityFramework");
            LoadFrameworkBundle("UnityDriver");
            LoadFrameworkBundle("VuforiaEngine");

            unity.SetDataBundleId("com.unity3d.framework");

            using var empty = new NSDictionary();
            unity.RunEmbeddedWithArgc(1, new string[1] { "InitializeUnity" }, empty);

            HideUnityWindowFromForeground();

            IsInitialized = true;
#endif
        }

        void ShowAppWindowFromBackground()
        {
            ShowWindow(appWindow);
        }

        void HideAppWindowFromForeground()
        {
            HideWindow(appWindow);
        }

        void ShowUnityWindowFromBackground()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && UnityWindow.WindowScene == null)
            {
                UnityWindow.WindowScene = appWindow.WindowScene;
            }

            ShowWindow(UnityWindow);
        }

        void HideUnityWindowFromForeground()
        {
            HideWindow(UnityWindow);
        }

        void HideWindow(UIWindow window)
        {
            window.Alpha = 1;
            window.Hidden = false;
            PerformAnimation(() => window.Alpha = 0);
        }

        void ShowWindow(UIWindow window)
        {
            window.Alpha = 0;
            window.Hidden = false;
            PerformAnimation(() => window.Alpha = 1);
        }

        void PerformAnimation(Action action, double duration = 1, [CallerMemberName] string caller = null)
        {
            UIView.BeginAnimations(caller);
            UIView.SetAnimationDuration(duration);
            action.Invoke();
            UIView.CommitAnimations();
        }

        void LoadFrameworkBundle(string frameworkName)
        {
            using var bundle = new NSBundle($"{NSBundle.MainBundle.BundlePath}/Frameworks/{frameworkName}.framework");

            if (!bundle.IsLoaded)
            {
                bundle.Load();
            }
        }

        UIWindow RetrieveWindow(bool keyWindow)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                return UIApplication.SharedApplication.Windows.FirstOrDefault(win => keyWindow == win.IsKeyWindow);
            }

            var scenes = UIApplication.SharedApplication.ConnectedScenes;

            foreach (UIScene scene in scenes)
            {
                if (scene.ActivationState != UISceneActivationState.ForegroundActive)
                {
                    continue;
                }

                if (scene is not UIWindowScene windowScene)
                {
                    continue;
                }

                var windows = windowScene.Windows;

                foreach (var window in windows)
                {
                    if (window.IsKeyWindow && keyWindow)
                    {
                        return window;
                    }
                    else if (!window.IsKeyWindow && !keyWindow)
                    {
                        return window;
                    }
                }
            }

            return null;
        }
    }
}
