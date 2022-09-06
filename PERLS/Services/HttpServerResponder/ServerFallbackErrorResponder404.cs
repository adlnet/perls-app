using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using Float.HttpServer;
using PERLS.DataImplementation;
using Xamarin.Forms;

namespace PERLS.Services.HttpServerResponder
{
    /// <summary>
    /// This serves as a package "polyfill".
    /// -- If a packaged piece of content requests a resource that is not available in the package,
    /// -- then this will request the resource from the remote server.
    /// Otherwise creates a 404 response with hidden error message.
    /// </summary>
    public class ServerFallbackErrorResponder404 : ErrorResponder404
    {
        readonly DrupalAPI drupalAPI = DependencyService.Get<DrupalAPI>();

        /// <inheritdoc/>
        public override void GenerateErrorResponse(in HttpListenerRequest httpRequest, ref HttpListenerResponse httpResponse, Exception e)
        {
            if (httpResponse == null || e == null || httpResponse.OutputStream == null || httpResponse.ContentLength64 != 0)
            {
                return;
            }

            try
            {
                GetRemoteServerResponse(httpRequest, ref httpResponse);
            }
            catch
            {
            }

            base.GenerateErrorResponse(httpRequest, ref httpResponse, e);
        }

        static IEnumerable<string> RemoteWhiteList()
        {
            yield return @"^/block-ajax-load";
            yield return @"^/history/\d+/read$";
            yield return @"^/node/\d+/render-stats$";
            yield return @"^/core/modules/statistics/statistics.php$";
            yield return @"^/sites/default/files/css/css_((?!/).)+\.css$";
            yield return @"^/sites/default/files/.+\.(jpg|png)$";
            yield return @"^/flag/flag/\w+/\d+$";
            yield return @"^/flag/details/flag/\w+/\d+$";
            yield return @"^/flag/details/delete/\w+/\d+$";
        }

        static IEnumerable<string> RemoteBlackListHeaderKeys()
        {
            yield return @"Set-Cookie";
            yield return @"Transfer-Encoding";
        }

        bool IsRequestWhiteListed(HttpListenerRequest httpRequest) => RemoteWhiteList().Any(regex => Regex.IsMatch(httpRequest.Url.LocalPath, regex));

        bool ShouldNotCopyHeader(string headerKey) => RemoteBlackListHeaderKeys().Any(key => key == headerKey);

        void GetRemoteServerResponse(HttpListenerRequest httpRequest, ref HttpListenerResponse httpResponse)
        {
            if (!IsRequestWhiteListed(httpRequest))
            {
                throw new Exception("Route is not whitelisted.");
            }

            var contentType = MediaTypeHeaderValue.TryParse(httpRequest.ContentType, out var contentMediaType) ? contentMediaType.MediaType : "application/json";
            var query = ParseQueryString(httpRequest.Url.Query);
            var body = httpRequest.InputStream == Stream.Null ? null : httpRequest.InputStream.ToString();
            var response = drupalAPI.RawResponse(httpRequest.HttpMethod, httpRequest.Url.LocalPath, query, body, contentType).Result;
            httpResponse.StatusCode = (int)response.StatusCode;
            var headers = response.HttpResponse.Headers;
            foreach (var header in headers)
            {
                if (ShouldNotCopyHeader(header.Key))
                {
                    continue;
                }

                httpResponse.AddHeader(header.Key, header.Value.FirstOrDefault());
            }

            if (httpRequest.HttpMethod == "POST")
            {
                httpResponse.AddHeader("X-Drupal-Ajax-Token", "1");
            }

            httpResponse.ContentType = response.HttpResponse.Content.Headers.ContentType.ToString();
            var content = response.HttpResponse.Content.ReadAsStreamAsync().Result;
            httpResponse.ContentLength64 = content.Length;
            content.CopyToAsync(httpResponse.OutputStream).Wait();
        }

        Dictionary<string, string> ParseQueryString(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return null;
            }

            var query = HttpUtility.ParseQueryString(queryString);
            return query.AllKeys.ToDictionary(k => k, k => query[k]);
        }
    }
}
