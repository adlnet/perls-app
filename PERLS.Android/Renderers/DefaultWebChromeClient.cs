using System;
using Android.Views;
using Xamarin.Forms.Platform.Android;

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// The default web chrome client.
    /// </summary>
    public class DefaultWebChromeClient : FormsWebChromeClient
    {
        /// <summary>
        /// Triggered when the content requests full-screen.
        /// </summary>
        public event EventHandler<EnterFullScreenRequestedEventArgs> EnterFullscreenRequested;

        /// <summary>
        /// Triggered when the content requests exiting full-screen.
        /// </summary>
        public event EventHandler ExitFullscreenRequested;

        /// <inheritdoc />
        public override void OnHideCustomView()
        {
            ExitFullscreenRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public override void OnShowCustomView(View view, ICustomViewCallback callback)
        {
            EnterFullscreenRequested?.Invoke(this, new EnterFullScreenRequestedEventArgs(view));
        }
    }
}
