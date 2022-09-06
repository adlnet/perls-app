using PERLS.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ScrollView), typeof(DefaultScrollViewRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Custom scrollview renderer to remove the additional content inset added by Shell.
    /// </summary>
    /// <remarks>
    /// When a scrollview is shown on a page that is part of a Shell, Xamarin Forms will adjust the
    /// content insets to take into account the top toolbar and current safe area.
    /// Unfortunately, the implementation makes wild assumptions about the context in which the scrollview
    /// is shown and provides no mechanism to adjust or opt out of that behavior. This results in scrollviews
    /// with unnecessary padding at the top on some devices. Since we can't opt out, we are forced to remove
    /// the unnecessary insets on every layout pass.
    ///
    /// At the time of writing this, only the learner goals/insights page is impacted by this.
    /// </remarks>
    /// <see href="https://github.com/xamarin/Xamarin.Forms/blob/caab66bcf9614aca0c0805d560a34e176d196e17/Xamarin.Forms.Platform.iOS/Renderers/ShellScrollViewTracker.cs"/>
    public class DefaultScrollViewRenderer : ScrollViewRenderer
    {
        bool isInAShell;

        /// <inheritdoc />
        public override void LayoutSubviews()
        {
            if (isInAShell)
            {
                ContentInset = UIEdgeInsets.Zero;
            }

            base.LayoutSubviews();
        }

        /// <inheritdoc />
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            // Determine if the element is in a Shell.
            isInAShell = false;
            var parent = Element.Parent;
            while (parent != null)
            {
                if (parent is ShellSection)
                {
                    isInAShell = true;
                    break;
                }

                parent = parent.Parent;
            }
        }
    }
}
