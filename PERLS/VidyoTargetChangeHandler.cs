using PERLS.Data;
using Xamarin.Forms;

namespace PERLS
{
    /// <summary>
    /// The Vidyo link target changer/fixer/mutator.
    /// </summary>
    public class VidyoTargetChangeHandler : IJavaScriptActionHandler
    {
        /// <inheritdoc />
        public string AttachedJavaScript => @"
        (function() {
            if ('undefined' === typeof jQuery) {
                return;
            }

            jQuery(document).ready(function() {
                jQuery(""a[target = 'vidyo']"").attr(""target"", ""_self"");
            });
        })();";

        /// <inheritdoc />
        public string ActionName => "vidyo_link_intercept";

        /// <inheritdoc />
        public void PerformAction(Effect element, object data)
        {
        }
    }
}
