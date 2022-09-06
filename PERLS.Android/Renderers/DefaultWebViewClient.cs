using System;
using Android.Runtime;
using Android.Webkit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace PERLS.Droid.Renderers
{
    /// <summary>
    /// The default web view client.
    /// </summary>
    /// <remarks>
    /// Heads up: This default webview client is usually replaced with <see cref="Effects.JSFormsWebViewClient"/>.
    /// It might be worth either combining these clients or having one subclass the other.
    /// </remarks>
    public class DefaultWebViewClient : FormsWebViewClient
    {
        WebViewRenderer renderer;

        /// <summary>
        /// Flag to prevent the app from accessing a base object if it has been disposed.
        /// </summary>
        bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebViewClient"/> class.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public DefaultWebViewClient(WebViewRenderer renderer) : base(renderer)
        {
            this.renderer = renderer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebViewClient"/> class.
        /// </summary>
        /// <param name="javaReference">A IntPtr containing a Java Native Interface (JNI) object reference.</param>
        /// <param name="transfer">A JniHandleOwnership indicating how to handle javaReference.</param>
        /// <remarks>
        /// A constructor used when creating managed representations of JNI objects; called by the runtime.
        /// In C#, derived classes cannot inherit the constructor of their base class.
        /// This constructor seems to be called by some internal Android process (not Forms)
        /// and thus must be defined on our subclass of FormsWebViewClient.
        /// </remarks>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/android.webkit.webviewclient.-ctor?view=xamarin-android-sdk-9#Android_Webkit_WebViewClient__ctor_System_IntPtr_Android_Runtime_JniHandleOwnership_"/>
        protected DefaultWebViewClient(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        /// <inheritdoc/>
        public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, IWebResourceRequest request)
        {
            if (request?.Method == "POST" && request.IsForMainFrame && renderer?.Element is Xamarin.Forms.WebView webView)
            {
                var url = new Uri(request.Url.ToString());
                webView.SendNavigating(new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url.AbsoluteUri }, url.AbsoluteUri));
            }

            if (isDisposed)
            {
                return null;
            }
            else
            {
                return base.ShouldInterceptRequest(view, request);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            isDisposed = true;
            base.Dispose(disposing);
            if (disposing)
            {
                renderer = null;
            }
        }
    }
}
