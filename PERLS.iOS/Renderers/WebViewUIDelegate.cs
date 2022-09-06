using System;
using System.Linq;
using CoreMotion;
using Foundation;
using PERLS.Data;
using UIKit;
using WebKit;

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// The WebKitUIDelegate.
    /// </summary>
    /// <remarks>
    /// Handling for JavaScript alerts is borrowed from https://github.com/xamarin/Xamarin.Forms/blob/5.0.0/Xamarin.Forms.Platform.iOS/Renderers/WkWebViewRenderer.cs#L104.
    /// </remarks>
    public class WebViewUIDelegate : WKUIDelegate
    {
        /// <inheritdoc/>
        public override void RequestDeviceOrientationAndMotionPermission(WKWebView webView, WKSecurityOrigin origin, WKFrameInfo frame, Action<WKPermissionDecision> decisionHandler)
        {
            using var manager = new CMMotionActivityManager();
            manager.QueryActivity(NSDate.Now, NSDate.Now, NSOperationQueue.MainQueue, (activities, error) =>
            {
                if (error == null)
                {
                    decisionHandler?.Invoke(WKPermissionDecision.Grant);
                }
                else
                {
                    decisionHandler?.Invoke(WKPermissionDecision.Prompt);
                }
            });
        }

        /// <inheritdoc/>
        public override void RunJavaScriptAlertPanel(WKWebView webView, string message, WKFrameInfo frame, Action completionHandler)
        {
            PresentAlertController(webView, message, okayAction: _ => completionHandler());
        }

        /// <inheritdoc/>
        public override void RunJavaScriptConfirmPanel(WKWebView webView, string message, WKFrameInfo frame, Action<bool> completionHandler)
        {
            PresentAlertController(webView, message, okayAction: _ => completionHandler(true), cancelAction: _ => completionHandler(false));
        }

        /// <inheritdoc/>
        public override void RunJavaScriptTextInputPanel(WKWebView webView, string prompt, string defaultText, WKFrameInfo frame, Action<string> completionHandler)
        {
            PresentAlertController(webView, prompt, defaultText, alert => completionHandler(alert.TextFields.FirstOrDefault()?.Text), _ => completionHandler(null));
        }

        static string GetJsAlertTitle(WKWebView webView)
        {
            if (webView.Url != null && webView.Url.AbsoluteString != $"file://{NSBundle.MainBundle.BundlePath}/")
            {
                return $"{webView.Url.Scheme}://{webView.Url.Host}";
            }

            using var title = new NSString(NSBundle.MainBundle.BundlePath);
            return title.LastPathComponent;
        }

        static void AddOkAction(UIAlertController controller, Action handler)
        {
            using var action = UIAlertAction.Create(Strings.Okay, UIAlertActionStyle.Default, _ => handler());
            controller.AddAction(action);
            controller.PreferredAction = action;
        }

        static void AddCancelAction(UIAlertController controller, Action handler)
        {
            using var action = UIAlertAction.Create(Strings.Cancel, UIAlertActionStyle.Cancel, _ => handler());
            controller.AddAction(action);
        }

        static void PresentAlertController(
            WKWebView webView,
            string message,
            string defaultText = null,
            Action<UIAlertController> okayAction = null,
            Action<UIAlertController> cancelAction = null)
        {
            using var controller = UIAlertController.Create(GetJsAlertTitle(webView), message, UIAlertControllerStyle.Alert);

            if (defaultText != null)
            {
                controller.AddTextField(textField => textField.Text = defaultText);
            }

            if (okayAction != null)
            {
                AddOkAction(controller, () => okayAction(controller));
            }

            if (cancelAction != null)
            {
                AddCancelAction(controller, () => cancelAction(controller));
            }

            GetTopViewController(UIApplication.SharedApplication.KeyWindow.RootViewController)
                .PresentViewController(controller, true, null);
        }

        static UIViewController GetTopViewController(UIViewController viewController)
        {
            if (viewController is UINavigationController navigationController)
            {
                return GetTopViewController(navigationController.VisibleViewController);
            }

            if (viewController is UITabBarController tabBarController)
            {
                return GetTopViewController(tabBarController.SelectedViewController);
            }

            if (viewController.PresentedViewController != null)
            {
                return GetTopViewController(viewController.PresentedViewController);
            }

            return viewController;
        }
    }
}
