using System;
using System.Threading.Tasks;
using Foundation;
using PERLS.Services;
using SafariServices;
using UIKit;
using Xamarin.Essentials;

namespace PERLS.iOS
{
    /// <summary>
    /// A BrowserService implementation for iOS.
    /// </summary>
    public class BrowserService : SFSafariViewControllerDelegate, IBrowserService
    {
        /// <inheritdoc/>
        public async Task<bool> OpenBrowser(Uri uri, BrowserLaunchOptions options)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            switch (options.LaunchMode)
            {
                case BrowserLaunchMode.SystemPreferred:
                    var nativeUrl = new NSUrl(urlString: uri.AbsoluteUri);
                    var safariViewController = new SFSafariViewController(nativeUrl, false);
                    safariViewController.Delegate = this;
                    var vc = Xamarin.Essentials.Platform.GetCurrentUIViewController();

                    if (options.PreferredToolbarColor.HasValue)
                    {
                        safariViewController.PreferredBarTintColor = options.PreferredToolbarColor.Value.ToPlatformColor();
                    }

                    if (options.PreferredControlColor.HasValue)
                    {
                        safariViewController.PreferredControlTintColor = options.PreferredControlColor.Value.ToPlatformColor();
                    }

                    if (safariViewController.PopoverPresentationController != null)
                    {
                        safariViewController.PopoverPresentationController.SourceView = vc.View;
                    }

                    if (options.Flags.HasFlag(BrowserLaunchFlags.PresentAsFormSheet))
                    {
                        safariViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    }
                    else if (options.Flags.HasFlag(BrowserLaunchFlags.PresentAsPageSheet))
                    {
                        safariViewController.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
                    }

                    await vc.PresentViewControllerAsync(safariViewController, true).ConfigureAwait(false);

                    nativeUrl.Dispose();
                    safariViewController.Dispose();
                    break;
                case BrowserLaunchMode.External:
                default:
                    return await Browser.OpenAsync(uri, options).ConfigureAwait(false);
            }

            return true;
        }

        /// <inheritdoc/>
        public override void DidFinish(SFSafariViewController controller)
        {
            if (controller == null)
            {
                return;
            }

            controller.Dispose();
        }
    }
}
