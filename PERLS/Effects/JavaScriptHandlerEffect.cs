using System.Collections.Generic;
using PERLS.Data;
using Xamarin.Forms;

namespace PERLS.Effects
{
    /// <summary>
    /// Handles linking JS to C# in WebView.
    /// </summary>
    public class JavaScriptHandlerEffect : RoutingEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptHandlerEffect"/> class.
        /// </summary>
        public JavaScriptHandlerEffect() : base("PERLS.JavaScriptHandlerEffect")
        {
        }

        /// <summary>
        /// Gets or sets the JavaScript Action Handlers.
        /// </summary>
        /// <value>A list of JavaScript to C# actions.</value>
        public List<IJavaScriptActionHandler> Handlers { get; set; }

        /// <summary>
        /// Triggered from JavaScript,
        /// this takes a dictionary (containing an action and data)
        /// and performs the action.
        /// </summary>
        /// <param name="effect">The effect that invoked the method.</param>
        /// <param name="action">The action and data sent from JavaScript.</param>
        public void SendAction(Effect effect, Dictionary<string, dynamic> action)
        {
            if (action == null)
            {
                return;
            }

            var actionName = action["action"] as string;
            var data = (object)action["data"];
            if (string.IsNullOrEmpty(actionName) || data == null)
            {
                return;
            }

            try
            {
                var handler = Handlers.Find((javaScripthandler) => actionName == javaScripthandler.ActionName);
                handler?.PerformAction(effect, data);
            }
            catch
            {
            }
        }
    }
}
