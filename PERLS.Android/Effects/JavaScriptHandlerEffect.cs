using System;
using System.Collections.Generic;
using System.Linq;
using PERLS.Data.Infrastructure;
using PERLS.Data.ViewModels;
using PERLS.Droid.Effects;
using PERLS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(AndroidJavaScriptHandlerEffect), nameof(JavaScriptHandlerEffect))]

namespace PERLS.Droid.Effects
{
    /// <summary>
    /// Enables a C# to be invoked from JavaScript.
    /// </summary>
    public sealed class AndroidJavaScriptHandlerEffect : PlatformEffect, IDisposable
    {
        /// <summary>
        /// Gets the name of the JavaScript interface.
        /// </summary>
        const string JavaScriptBridge = "jsBridge";

        /// <summary>
        /// Gets the required JavaScript to attach to bridge between JS and C#.
        /// </summary>
        readonly string attachedJavaScript = $@"
        function invokeCSharpWebFormSubmit(data){{{JavaScriptBridge}.invokeAction(data);}}
        ";

        JavaScriptHandlerEffectDelegate javaScriptBridge;
        JSFormsWebViewClient webViewClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidJavaScriptHandlerEffect"/> class.
        /// </summary>
        public AndroidJavaScriptHandlerEffect()
            : base()
        {
            javaScriptBridge = new JavaScriptHandlerEffectDelegate(this);
        }

        /// <summary>
        /// Gets the "parent" JavaScript handler effect.
        /// </summary>
        JavaScriptHandlerEffect JavaScriptHandlerEffect => Element?.Effects.FirstOrDefault(e => e is JavaScriptHandlerEffect) as JavaScriptHandlerEffect;

        /// <inheritdoc />
        public void Dispose()
        {
            CleanUp();
        }

        /// <summary>
        /// Method to call parent to send xAPI statement.
        /// </summary>
        internal void SendAction(Dictionary<string, dynamic> data)
        {
            JavaScriptHandlerEffect.SendAction(this, data);
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            if (JavaScriptHandlerEffect.Handlers == null)
            {
                return;
            }

            if (webViewClient == null &&
                Container is Renderers.WebViewRenderer renderer &&
                renderer.Control is Android.Webkit.WebView control)
            {
                var attachments = JavaScriptHandlerEffect.Handlers.Select(o => o.AttachedJavaScript)
                                                                  .ToArray();
                var javaScriptActions = string.Join("\n", attachments);
                var javaScript = $"{attachedJavaScript} \n {javaScriptActions}";
                webViewClient = new JSFormsWebViewClient(renderer, javaScript);
                control.AddJavascriptInterface(javaScriptBridge, JavaScriptBridge);
                control.SetWebViewClient(webViewClient);
                control.LoadUrl(control.Url);
            }
        }

        /// <inheritdoc />
        protected override void OnDetached()
        {
            CleanUp();
        }

        /// <summary>
        /// Cleans + Disposes Elements.
        /// </summary>
        void CleanUp()
        {
            if (this.Container is Renderers.WebViewRenderer renderer)
            {
                renderer.Control.RemoveJavascriptInterface(JavaScriptBridge);
                javaScriptBridge.Dispose();
            }

            if (webViewClient != null)
            {
                webViewClient.Dispose();
            }
        }
    }
}
