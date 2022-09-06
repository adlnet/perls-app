using System;
using Android.Views;

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// Event arguments for a WebView request to go full-screen.
    /// </summary>
    public class EnterFullScreenRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnterFullScreenRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="view">The Android view that should be displayed in full-screen.</param>
        public EnterFullScreenRequestedEventArgs(View view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            View = view;
        }

        /// <summary>
        /// Gets the Android view that is to be displayed in full-screen.
        /// </summary>
        /// <value>
        /// The Android view that is to be displayed in full-screen.
        /// </value>
        public View View { get; }
    }
}
