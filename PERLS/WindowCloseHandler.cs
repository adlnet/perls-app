using System;
using PERLS.Data;
using Xamarin.Forms;

namespace PERLS
{
    /// <summary>
    /// Intercepts requests to close the window so that the view
    /// can instead be popped off the navigation stack.
    /// </summary>
    public class WindowCloseHandler : IJavaScriptActionHandler
    {
        readonly INavigation navigation;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCloseHandler"/> class.
        /// </summary>
        /// <param name="navigation">The current navigation stack.</param>
        public WindowCloseHandler(INavigation navigation)
        {
            this.navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        }

        /// <inheritdoc />
        public string AttachedJavaScript => @"
window.close = function() {
    var data = {
        action: ""window_close_intercept"",
        data: {}
    };
    invokeCSharpWebFormSubmit(JSON.stringify(data));
};
";

        /// <inheritdoc />
        public string ActionName => "window_close_intercept";

        /// <inheritdoc />
        public void PerformAction(Effect element, object data)
        {
            Device.BeginInvokeOnMainThread(() => navigation.PopAsync());
        }
    }
}
