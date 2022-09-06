using System;
using System.Threading.Tasks;
using Foundation;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.iOS;
using PERLS.Services;
using UIKit;
using WebKit;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(DocumentOpener))]

namespace PERLS.iOS
{
    /// <summary>
    /// The iOS implementation of the Document Opener.
    /// </summary>
    public class DocumentOpener : IDocumentOpener
    {
        IFile file;

        /// <inheritdoc/>
        public Task OpenAsync(IFile file)
        {
            this.file = file ?? throw new ArgumentNullException(nameof(file));
            var filename = file.Name;
            var filepath = file.LocalPath;
            var vc = Platform.GetCurrentUIViewController();

            // Create a WKWebView
            var configuration = new WKWebViewConfiguration();
            var webView = new WKWebView(vc.View.Frame, configuration);
            var fileUrl = NSUrl.FromFilename(filepath);
            webView.LoadFileUrl(fileUrl, fileUrl);

            // Add it to a navigation
            var viewController = new UIViewController
            {
                View = webView,
            };

            // Add an "Open in" button
            var buttonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action);
            buttonItem.Clicked += SharePressed;

            UIBarButtonItem closeItem;

#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                var appearance = new UINavigationBarAppearance();
                appearance.ConfigureWithOpaqueBackground();
                viewController.NavigationItem.StandardAppearance = appearance;
                viewController.NavigationItem.ScrollEdgeAppearance = appearance;
                closeItem = new UIBarButtonItem(UIBarButtonSystemItem.Close);
            }
#pragma warning restore XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
            else
            {
                closeItem = new UIBarButtonItem(Strings.CloseLabel, UIBarButtonItemStyle.Plain, null, null);
            }

            closeItem.Clicked += ClosePressed;

            viewController.NavigationItem.LeftBarButtonItem = closeItem;
            viewController.NavigationItem.RightBarButtonItem = buttonItem;
            viewController.NavigationItem.Title = filename;

            var parentViewController = vc.ParentViewController;
            while (!(parentViewController is UINavigationController) && parentViewController != null)
            {
                parentViewController = parentViewController.ParentViewController;
            }

            if (parentViewController is UINavigationController currentNavigationController)
            {
                currentNavigationController.PushViewController(viewController, true);
            }
            else
            {
                var navigationController = new UINavigationController(viewController);
                vc.PresentViewController(navigationController, true, null);
                navigationController.Dispose();
            }

            configuration.Dispose();
            webView.Dispose();
            viewController.Dispose();

            return Task.CompletedTask;
        }

        void SharePressed(object sender, EventArgs args)
        {
            PresentOpener(file);
        }

        void ClosePressed(object sender, EventArgs args)
        {
            var vc = Platform.GetCurrentUIViewController();
            vc.DismissModalViewController(true);
        }

        void PresentOpener(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var fileUrl = NSUrl.FromFilename(file.LocalPath);

            UIDocumentInteractionController documentController = UIDocumentInteractionController.FromUrl(fileUrl);
            documentController.Name = file.Name;

            var vc = Platform.GetCurrentUIViewController();

            CoreGraphics.CGRect rect;

            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                rect = new CoreGraphics.CGRect(new CoreGraphics.CGPoint(vc.View.Bounds.Width / 2, vc.View.Bounds.Height), CoreGraphics.CGRect.Empty.Size);
            }
            else
            {
                rect = vc.View.Bounds;
            }

            documentController.PresentOpenInMenu(rect, vc.View, true);
        }
    }
}
