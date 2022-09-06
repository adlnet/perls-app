using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using PERLS.Effects;
using PERLS.iOS.Effects;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(AppleJavaScriptHandlerEffect), nameof(JavaScriptHandlerEffect))]

namespace PERLS.iOS.Effects
{
    /// <summary>
    /// Enables a C# to be invoked from JavaScript.
    /// </summary>
    public sealed class AppleJavaScriptHandlerEffect : PlatformEffect, IDisposable
    {
        /// <summary>
        /// Gets the required JavaScript to attach to bridge between JS and C#.
        /// </summary>
        readonly string attachedJavaScript = @"
        function invokeCSharpWebFormSubmit(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}
        ";

        /// <summary>
        /// The user controller which to attach the user script.
        /// </summary>
        WKUserContentController userController;

        /// <summary>
        /// The message delegate that handles when C# is invoked from JS to C#.
        /// </summary>
        JavaScriptHandlerEffectDelegate messageDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleJavaScriptHandlerEffect"/> class.
        /// </summary>
        public AppleJavaScriptHandlerEffect()
        {
            messageDelegate = new JavaScriptHandlerEffectDelegate(this);
        }

        /// <summary>
        /// Gets the "parent" JavaScript handler effect.
        /// </summary>
        JavaScriptHandlerEffect JavaScriptHandlerEffect => Element?.Effects.FirstOrDefault(e => e is JavaScriptHandlerEffect) as JavaScriptHandlerEffect;

        /// <inheritdoc />
        public void Dispose()
        {
            CleanUp();
            messageDelegate.Dispose();
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

            if (this.Control is WKWebView webView)
            {
                var config = webView.Configuration;
                userController = config.UserContentController;
                CleanUp();
                var attachments = JavaScriptHandlerEffect.Handlers.Select(o => o.AttachedJavaScript)
                                                                  .ToArray();
                string javaScriptActions = string.Join("\n", attachments);

                // Only add once... for some reason OnDetached isn't called
                // when the webview goes away
                if (userController.UserScripts.Length == 0)
                {
                    using (var javaScriptFunction = new NSString($"{attachedJavaScript} \n {javaScriptActions}"))
                    using (var script = new WKUserScript(javaScriptFunction, WKUserScriptInjectionTime.AtDocumentEnd, false))
                    {
                        this.userController.AddUserScript(script);
                        this.userController.AddScriptMessageHandler(messageDelegate, "invokeAction");
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnDetached()
        {
            CleanUp();
        }

        /// <summary>
        /// Method to clean up user scripts from web view.
        /// </summary>
        void CleanUp()
        {
            userController?.RemoveAllUserScripts();
            userController?.RemoveScriptMessageHandler("invokeAction");
        }
    }
}
