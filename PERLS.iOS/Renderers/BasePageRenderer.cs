using System;
using Foundation;
using PERLS.iOS.Renderers;
using PERLS.Pages;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BasePage), typeof(BasePageRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Base page renderer.
    /// </summary>
    public class BasePageRenderer : PageRenderer
    {
        readonly WeakReference<BasePage> weakPage = new WeakReference<BasePage>(null);

        /// <inheritdoc />
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Device.BeginInvokeOnMainThread(() =>
            {
                if (Element is BasePage page && page.AllowLandscape)
                {
                    return;
                }

                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));
            });
        }

        /// <inheritdoc/>
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (Element is BasePage page)
            {
                page.OnAppeared();
            }
        }

        /// <inheritdoc/>
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (Element is BasePage page)
            {
                page.OnDisappeared();
                weakPage.SetTarget(page);
            }
        }

        /// <inheritdoc />
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);

            if (Element is BasePage page && page.HidesBarsInLandscape && toInterfaceOrientation.IsLandscape())
            {
                ExtendedLayoutIncludesOpaqueBars = true;
            }
            else
            {
                ExtendedLayoutIncludesOpaqueBars = false;
            }
        }

        /// <inheritdoc/>
        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            if (Element is BasePage page)
            {
                page.OnFinishedRotation();
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (weakPage.TryGetTarget(out var page))
            {
                page.OnUnload();
            }
        }
    }
}
