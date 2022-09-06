using System;
using System.Collections.Generic;
using Float.Core.Analytics;
using Foundation;
using Newtonsoft.Json;
using WebKit;
using Xamarin.Forms;

namespace PERLS.iOS.Effects
{
    /// <summary>
    /// Handles messages from JS for iOS.
    /// </summary>
    public class JavaScriptHandlerEffectDelegate : NSObject, IWKScriptMessageHandler
    {
        /// <summary>
        /// Reference to the Effect that sets up JS script.
        /// </summary>
        WeakReference<AppleJavaScriptHandlerEffect> javaScriptHandlerEffect;

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptHandlerEffectDelegate"/> class.
        /// </summary>
        /// <param name="effect">The JavaScript Handler Effect.</param>
        public JavaScriptHandlerEffectDelegate(AppleJavaScriptHandlerEffect effect)
        {
            this.javaScriptHandlerEffect = new WeakReference<AppleJavaScriptHandlerEffect>(effect);
        }

        /// <inheritdoc/>
        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (message == null)
            {
                return;
            }

            try
            {
                var responseJSON = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(message.Body.ToString());
                this.javaScriptHandlerEffect.TryGetTarget(out AppleJavaScriptHandlerEffect target);
                target.SendAction(responseJSON);
            }
            #pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            #pragma warning restore CA1031 // Do not catch general exception types
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }
        }
    }
}
