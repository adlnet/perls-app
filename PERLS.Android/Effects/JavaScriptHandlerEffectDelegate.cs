using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Android.Runtime;
using Android.Webkit;
using Float.Core.Analytics;
using Java.Interop;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PERLS.Droid.Effects
{
    /// <summary>
    /// A bridge to the webview to webview client to run JavaScript.
    /// </summary>
    public class JavaScriptHandlerEffectDelegate : Java.Lang.Object
    {
        /// <summary>
        /// Reference to the Effect that sets up JS script.
        /// </summary>
        WeakReference<AndroidJavaScriptHandlerEffect> javaScriptHandlerEffect = new WeakReference<AndroidJavaScriptHandlerEffect>(default);

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptHandlerEffectDelegate"/> class.
        /// </summary>
        /// <param name="effect">The Xamarin effect to handle JavaScript.</param>
        public JavaScriptHandlerEffectDelegate(AndroidJavaScriptHandlerEffect effect)
        {
            javaScriptHandlerEffect.SetTarget(effect);
        }

        /// <summary>
        /// This is the C# method invoked from JS.
        /// </summary>
        /// <param name="data">The data from JS.</param>
        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            try
            {
                var responseJSON = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(data);
                this.javaScriptHandlerEffect.TryGetTarget(out AndroidJavaScriptHandlerEffect target);
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
