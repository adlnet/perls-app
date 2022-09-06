using System;
using Newtonsoft.Json.Linq;
using PERLS.Data;
using Xamarin.Forms;

namespace PERLS
{
    /// <summary>
    /// Intercepts Ajax links within an article and handles
    /// them has normal links so the app can present it's own modal
    /// instead of using the built-in Drupal modal.
    /// </summary>
    /// <remarks>
    /// This is used on the "View comments" button to access
    /// the public discussion feature.
    /// </remarks>
    public class AjaxLinkActionHandler : IJavaScriptActionHandler
    {
        /// <inheritdoc />
        public string AttachedJavaScript => @"
(function() {
    if ('undefined' === typeof jQuery) {
        return;
    }
    jQuery('a.use-ajax')
        .off('click')
        .on('click', function(event) {
            var $el = jQuery(this);
            var dialogOptions = JSON.parse($el.attr('data-dialog-options'));
            var url = this.href;
            var data = {
                action: ""ajax_link_intercept"",
                data: { url, dialogOptions }
            };
            invokeCSharpWebFormSubmit(JSON.stringify(data));
            event.stopImmediatePropagation();
            event.preventDefault();
        });
})();
";

        /// <inheritdoc />
        public string ActionName => "ajax_link_intercept";

        /// <inheritdoc />
        public void PerformAction(Effect element, object data)
        {
            // A good enhancement here would be to create "web view overlays"
            // directly from here referencing the "dialogOptions" that were
            // in the original markup. Since public discussion is the only
            // feature that uses an Ajax link within the article, we're
            // taking the simple path and just routing the link through
            // the application URI handler.
            if (data is JObject actionData)
            {
                var url = actionData["url"]?.ToString();
                var uri = new Uri(url);
                Application.Current.OpenUri(uri);
            }
        }
    }
}
