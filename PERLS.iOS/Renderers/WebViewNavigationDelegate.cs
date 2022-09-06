using System;
using Foundation;
using WebKit;
using Xamarin.Forms;

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// A modified version of the navigation delegate used in Xamarin Forms.
    /// </summary>
    public class WebViewNavigationDelegate : WKNavigationDelegate
    {
        readonly WebView xamWebView;
        readonly WebViewRenderer renderer;
        WebNavigationEvent lastEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewNavigationDelegate"/> class.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="webView">the xamarin webview.</param>
        public WebViewNavigationDelegate(WebViewRenderer renderer, WebView webView)
        {
            this.xamWebView = webView ?? throw new ArgumentNullException(nameof(webView));
            this.renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        /// <inheritdoc/>
        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            // If the error is code 204, that's because something else has handled it.
            // https://bugs.webkit.org/show_bug.cgi?id=60206
            if (error?.Domain == "WebKitErrorDomain" && error?.Code == 204)
            {
                // Sending as a success. This is perhaps somewhat misleading as it is more of a cancel than success, but any failures would be handled elsewhere.
                xamWebView.SendNavigated(
                new WebNavigatedEventArgs(lastEvent, new UrlWebViewSource { Url = GetCurrentUrl() }, GetCurrentUrl(), WebNavigationResult.Success));
                return;
            }

            var url = GetCurrentUrl();
            xamWebView.SendNavigated(
                new WebNavigatedEventArgs(lastEvent, new UrlWebViewSource { Url = url }, url, WebNavigationResult.Failure));
            renderer.UpdateCanGoBackForward();
        }

        /// <inheritdoc/>
        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            var url = GetCurrentUrl();
            xamWebView.SendNavigated(
                new WebNavigatedEventArgs(lastEvent, new UrlWebViewSource { Url = url }, url, WebNavigationResult.Failure));
            renderer.UpdateCanGoBackForward();
        }

        /// <inheritdoc/>
        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            if (webView == null || webView.IsLoading)
            {
                return;
            }

            var url = GetCurrentUrl();

            renderer.IgnoreSourceChanges = true;
            xamWebView.SetValueFromRenderer(WebView.SourceProperty, new UrlWebViewSource { Url = url });
            renderer.IgnoreSourceChanges = false;
            ProcessNavigated(url);
        }

        /// <inheritdoc/>
        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
        }

        /// <inheritdoc/>
        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            var navEvent = WebNavigationEvent.NewPage;
            var navigationType = navigationAction?.NavigationType;
            switch (navigationType)
            {
                case WKNavigationType.LinkActivated:
                    navEvent = WebNavigationEvent.NewPage;

                    if (navigationAction?.TargetFrame == null)
                    {
                        webView?.LoadRequest(navigationAction?.Request);
                    }

                    break;
                case WKNavigationType.FormSubmitted:
                    navEvent = WebNavigationEvent.NewPage;
                    break;
                case WKNavigationType.BackForward:
                    navEvent = renderer.LastBackForwardEvent;
                    break;
                case WKNavigationType.Reload:
                    navEvent = WebNavigationEvent.Refresh;
                    break;
                case WKNavigationType.FormResubmitted:
                    navEvent = WebNavigationEvent.NewPage;
                    break;
                case WKNavigationType.Other:
                    // This is the only significant change to Navigation Delegate
                    // if the request's main document url matches the target url, we are opening a new page
                    if (navigationAction?.Request?.Url == navigationAction?.Request?.MainDocumentURL)
                    {
                        navEvent = WebNavigationEvent.NewPage;
                    }
                    else
                    {
                        // otherwise, just allow navigation and bail
                        // this is to better handle resources, for which we may get navigating but not navigated events
                        decisionHandler?.Invoke(WKNavigationActionPolicy.Allow);
                        return;
                    }

                    break;
            }

            lastEvent = navEvent;
            var request = navigationAction?.Request;
            var lastUrl = request.Url.ToString();
            var args = new WebNavigatingEventArgs(navEvent, new UrlWebViewSource { Url = lastUrl }, lastUrl);

            xamWebView.SendNavigating(args);
            renderer.UpdateCanGoBackForward();

            decisionHandler?.Invoke(args.Cancel ? WKNavigationActionPolicy.Cancel : WKNavigationActionPolicy.Allow);
        }

        void ProcessNavigated(string url)
        {
            var args = new WebNavigatedEventArgs(lastEvent, xamWebView.Source, url, WebNavigationResult.Success);
            xamWebView.SendNavigated(args);
            renderer.UpdateCanGoBackForward();
        }

        string GetCurrentUrl()
        {
            return renderer?.Url?.AbsoluteUrl?.ToString();
        }
    }
}
