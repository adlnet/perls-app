using PERLS.Data;
using Xamarin.Forms;

namespace PERLS
{
    /// <summary>
    /// This changes tag links to a format that would be preferred.
    /// </summary>
    public class TagLinkChangeHandler : IJavaScriptActionHandler
    {
        /// <inheritdoc />
        public string AttachedJavaScript => @"
        (function() {
            function changeTagLink()
            {
                var links = document.querySelectorAll('a[sl-canonical-url]');
                var i;
                for (i = 0; i < links.length; i++) {
                    links[i].href = links[i].getAttribute('sl-canonical-url');
                }
            }

            if (document.readyState == 'loading') {
                document.addEventListener('DOMContentLoaded', changeTagLink);
            } else {
                changeTagLink();
            }

            if ('undefined' === typeof jQuery) {
                return;
            }

            jQuery(document).ready(function() {
                jQuery('a[sl-canonical-url]').click(function(event) {
                    window.location = jQuery(this).attr('sl-canonical-url');
                    event.stopImmediatePropagation();
                    event.preventDefault();
                });
            });
        })();";

        /// <inheritdoc />
        public string ActionName => "taglink_intercept";

        /// <inheritdoc />
        public void PerformAction(Effect element, object data)
        {
        }
    }
}
