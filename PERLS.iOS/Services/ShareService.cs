using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using PERLS.Services;
using UIKit;
using Xamarin.Essentials;

namespace PERLS.iOS.Services
{
    /// <summary>
    /// A service for sharing files.
    /// </summary>
    public class ShareService : IShareService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShareService"/> class.
        /// </summary>
        public ShareService()
        {
        }

        /// <inheritdoc />
        public Task RequestAsync(ShareFileRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var fileUrl = NSUrl.FromFilename(request.File.FullPath);
            NSObject item = string.IsNullOrWhiteSpace(request.Title) ? fileUrl : new ShareActivityItemSource(fileUrl, request.Title);

            using (var activityController = new UIActivityViewController(new[] { item }, null))
            {
                var viewController = UIApplication.SharedApplication.Windows.FirstOrDefault()?.RootViewController;

                if (activityController.PopoverPresentationController != null)
                {
                    if (viewController?.View is UIView view)
                    {
                        activityController.PopoverPresentationController.SourceView = view;
                    }

                    if (request.PresentationSourceBounds != Rectangle.Empty)
                    {
                        activityController.PopoverPresentationController.SourceRect = request.PresentationSourceBounds.ToPlatformRectangle();
                    }
                }

                return viewController?.PresentViewControllerAsync(activityController, true);
            }
        }

        class ShareActivityItemSource : UIActivityItemSource
        {
            readonly NSObject item;
            readonly string subject;

            internal ShareActivityItemSource(NSObject item, string subject)
            {
                this.item = item;
                this.subject = subject;
            }

            public override NSObject GetItemForActivity(UIActivityViewController activityViewController, NSString activityType) => item;

            public override NSObject GetPlaceholderData(UIActivityViewController activityViewController) => item;

            public override string GetSubjectForActivity(UIActivityViewController activityViewController, NSString activityType) => subject;
        }
    }
}
