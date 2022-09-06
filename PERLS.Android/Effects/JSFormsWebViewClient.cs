using System;
using Android.Runtime;
using Android.Webkit;
using PERLS.Droid.Effects;
using PERLS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(AndroidJavaScriptHandlerEffect), nameof(JavaScriptHandlerEffect))]

namespace PERLS.Droid.Effects
{
    /// <summary>
    /// A webview client which attaches given JavaScript.
    /// </summary>
    public class JSFormsWebViewClient : FormsWebViewClient
    {
        /// <summary>
        /// The JavaScript to attach to the page.
        /// </summary>
        readonly string attachedJavaScript;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSFormsWebViewClient"/> class.
        /// </summary>
        /// <param name="renderer">The webview renderer.</param>
        /// <param name="javaScript">The JavaScript to which to attach.</param>
        public JSFormsWebViewClient(WebViewRenderer renderer, string javaScript) : base(renderer)
        {
            this.attachedJavaScript = javaScript;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSFormsWebViewClient"/> class.
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
        protected JSFormsWebViewClient(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        /// <inheritdoc />
        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, IWebResourceRequest request)
        {
            // If this request is not for the main frame (i.e. it's for an iframe)
            // then don't bother intercepting or even alerting Xamarin.Forms to the loading event.
            // This ensures the loading indicator doesn't get stuck on screen.
            if (request?.IsForMainFrame != true)
            {
                return false;
            }

            return base.ShouldOverrideUrlLoading(view, request);
        }

        /// <inheritdoc/>
        public override void OnPageCommitVisible(Android.Webkit.WebView view, string url)
        {
            base.OnPageCommitVisible(view, url);
            view?.EvaluateJavascript(this.attachedJavaScript, null);
        }
    }
}
