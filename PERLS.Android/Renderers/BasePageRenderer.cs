using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using PERLS.Droid.Renderers;
using PERLS.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BasePage), typeof(BasePageRenderer))]

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// Base page renderer.
    /// </summary>
    public class BasePageRenderer : PageRenderer
    {
        Orientation latestOrientation;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePageRenderer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public BasePageRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc />
        protected override void OnVisibilityChanged(Android.Views.View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);

            if (Context is not FormsAppCompatActivity activity || Element is not BasePage page)
            {
                return;
            }

            activity.RequestedOrientation = page.AllowLandscape ? Android.Content.PM.ScreenOrientation.Unspecified : Android.Content.PM.ScreenOrientation.Portrait;

            switch (visibility)
            {
                case ViewStates.Visible:
                    page.OnAppeared();
                    break;
                case ViewStates.Gone:
                    page.OnDisappeared();
                    break;
                default:
                    break;
            }
        }

        /// <inheritdoc />
        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();

            if (Element is BasePage page)
            {
                page.OnUnload();
            }
        }

        /// <inheritdoc/>
        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            if (newConfig == null)
            {
                return;
            }

            if (latestOrientation != newConfig.Orientation)
            {
                latestOrientation = newConfig.Orientation;

                if (Element is BasePage page)
                {
                    page.OnFinishedRotation();
                }
            }
        }
    }
}
