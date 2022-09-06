using System;
using System.Reflection;
using ObjCRuntime;
using PERLS.Data.Definition.Services;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebView), typeof(PERLS.iOS.Renderers.WebViewRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Web view renderer.
    /// </summary>
    public class WebViewRenderer : WkWebViewRenderer
    {
        // Accessing the non-persistent data store at different parts of the application may not result in the same instance.
        // This makes cleaning up cookies on logout problematic, so we grab a reference here and use it for the configuration below.
        // This global data store also gets cleaned up in <see cref="ClearCookies"/>.
        static readonly WKWebsiteDataStore DataStore = WKWebsiteDataStore.NonPersistentDataStore;

        // WKWebpagePreferences is not available before iOS 13.
        static readonly bool HasWebpagePreferences = Type.GetType("WKWebViewConfiguration")?.GetProperty("WKWebpagePreferences") != null;

        /// <summary>
        /// The web configuration.
        /// </summary>
        /// <remarks>
        /// By using a non-persistent store, it grants us two benefits:
        /// 1. We don't share cookies with the rest of the app. All other HTTP requests should be using OAuth.
        /// 2. We don't persist data across application launches. The content should be storing state via xAPI (not via local storage or similar).
        /// </remarks>
        static readonly WKWebViewConfiguration WebConfiguration = HasWebpagePreferences ?
            new WKWebViewConfiguration
            {
                ApplicationNameForUserAgent = DependencyService.Get<IAppContextService>().UserAgentSuffix,
                AllowsInlineMediaPlayback = true,
                WebsiteDataStore = GlobalDataStore,
                ProcessPool = new WKProcessPool(),
                DefaultWebpagePreferences = new WKWebpagePreferences { PreferredContentMode = WKContentMode.Mobile },
            }
            :
            new WKWebViewConfiguration
            {
                ApplicationNameForUserAgent = DependencyService.Get<IAppContextService>().UserAgentSuffix,
                AllowsInlineMediaPlayback = true,
                WebsiteDataStore = GlobalDataStore,
                ProcessPool = new WKProcessPool(),
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewRenderer"/> class.
        /// </summary>
        public WebViewRenderer() : base(WebConfiguration)
        {
            // Allows full screen content to extend pass the safe areas of the screen,
            // but still account for safe areas if content needs to scroll.
            ScrollView.ContentInsetAdjustmentBehavior = UIKit.UIScrollViewContentInsetAdjustmentBehavior.ScrollableAxes;
        }

        /// <summary>
        /// Gets a reference to the global data store used when rendering web views.
        /// </summary>
        /// <value>The global datastore.</value>
        public static WKWebsiteDataStore GlobalDataStore => DataStore;

        /// <summary>
        /// Gets or sets a value indicating whether to ignore source changes.
        /// </summary>
        /// <value><c>true</c> if source changes should be ignored, <c>false</c> otherwise.</value>
        public bool IgnoreSourceChanges
        {
            get
            {
                var type = typeof(WkWebViewRenderer);
                var field = type.GetField("_ignoreSourceChanges", BindingFlags.NonPublic | BindingFlags.Instance);
                return (bool)field.GetValue(this);
            }

            set
            {
                var type = typeof(WkWebViewRenderer);
                var field = type.GetField("_ignoreSourceChanges", BindingFlags.NonPublic | BindingFlags.Instance);
                field.SetValue(this, value);
            }
        }

        /// <summary>
        /// Gets the last back or forward navigation event.
        /// </summary>
        /// <value>The last back or forward event.</value>
        public WebNavigationEvent LastBackForwardEvent
        {
            get
            {
                var type = typeof(WkWebViewRenderer);
                var field = type.GetField("_lastBackForwardEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                return (WebNavigationEvent)field.GetValue(this);
            }
        }

        /// <summary>
        /// Invoke to update whether this web view renderer can go back or forward.
        /// </summary>
        public void UpdateCanGoBackForward()
        {
            var type = typeof(WkWebViewRenderer);
            var method = type.GetMethod("UpdateCanGoBackForward", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(this, null);
        }

        /// <inheritdoc />
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e == null)
            {
                return;
            }

            this.NavigationDelegate = new WebViewNavigationDelegate(this, (WebView)Element);
            this.UIDelegate = new WebViewUIDelegate();
        }
    }
}
