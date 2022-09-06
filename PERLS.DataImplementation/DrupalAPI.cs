using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Float.Core.Analytics;
using Float.Core.Net;
using JsonNet.PrivateSettersContractResolvers;
using Newtonsoft.Json;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.DataImplementation.Models;
using PERLS.DataImplementation.Providers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.DataImplementation
{
    /// <summary>
    /// Request handler for interacting with the Drupal REST API.
    /// </summary>
    public class DrupalAPI : IRemoteProvider, IPushTokenProvider, IPagedResponseHandler
    {
        readonly INetworkConnectionService networkService = DependencyService.Get<INetworkConnectionService>();

        /// <summary>
        /// RequestClient for making user-authenticated requests to the API.
        /// This should be used for all requests after the user has logged in.
        /// </summary>
        RequestClient authenticatedRequestClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrupalAPI"/> class.
        /// </summary>
        public DrupalAPI()
        {
            UpdateRequestClient();
        }

        /// <summary>
        /// Gets a value indicating whether the server is reachable.
        /// </summary>
        /// <returns><c>true</c> if the server is reachable, <c>false</c> otherwise.</returns>
        public Task<bool> IsReachable() => networkService.IsReachable();

        /// <summary>
        /// Update the request client. Needed when the server uri is changed.
        /// </summary>
        public void UpdateRequestClient()
        {
            var authStrategy = networkService.AuthStrategy;

            var config = new ClientHandlerConfiguration
            {
                AllowCookies = false,
            };

            var handler = DependencyService.Get<INativeHttpClientHandler>().CreateHandler(config);

            var acceptHeader = new Dictionary<string, string>
            {
                { "accept", "application/json" },
            };

            authenticatedRequestClient = new RequestClient(networkService.BaseUri, authStrategy, acceptHeader, handler);
        }

        /// <summary>
        /// Gets a list of nodes.
        /// </summary>
        /// <param name="path">The API path.</param>
        /// <param name="query">An optional set of query parameters.</param>
        /// <returns>The node list.</returns>
        public async Task<IEnumerable<IItem>> GetNodeList(string path, Dictionary<string, string> query = null)
        {
            var nodes = await GetData<List<Node>>(path, query).ConfigureAwait(false);
            var stateProvider = DependencyService.Get<ILearnerStateProvider>();
            await stateProvider.GetStateList(nodes).ConfigureAwait(false);
            return nodes;
        }

        /// <summary>
        /// Gets the item states.
        /// </summary>
        /// <returns>The item states.</returns>
        /// <param name="nodes">The Nodes.</param>
        public Task<ItemStateGroup> GetItemStates(IEnumerable<Node> nodes)
        {
            return GetData<ItemStateGroup>("api/state/all", new Dictionary<string, string>
            {
                { "nid", string.Join("|", nodes.Select(node => node.Nid).ToArray()) },
            });
        }

        /// <summary>
        /// Gets the term states.
        /// </summary>
        /// <returns>The term states.</returns>
        /// <param name="terms">The terms.</param>
        public Task<TermStateGroup> GetTermStates(IEnumerable<ITaxonomyTerm> terms)
        {
            return GetData<TermStateGroup>("api/state/following", new Dictionary<string, string>
            {
                { "tid", string.Join("|", terms.Select(node => node.Tid).ToArray()) },
            });
        }

        /// <summary>
        /// Gets the remote resource given the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The remote resource.</returns>
        public async Task<IRemoteResource> GetRemoteResource(string path)
        {
            var response = await authenticatedRequestClient.Get(path).ConfigureAwait(false);
            var resource = JsonConvert.DeserializeObject<IRemoteResource>(response.Content, new RemoteResourceCustomCreationConverter());

            if (resource == null)
            {
                throw new DrupalCorpusProviderException("Remote resource unable to deserialize.");
            }

            return resource;
        }

        /// <summary>
        /// Retrieves and parses JSON from the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The type to parse the JSON into.</typeparam>
        /// <param name="path">The API path.</param>
        /// <param name="query">An optional set of query parameters.</param>
        /// <returns>The parsed data.</returns>
        public async Task<T> GetData<T>(string path, Dictionary<string, string> query = null)
        {
            var acceptHeader = new Dictionary<string, string>
            {
                { "Accept-Language", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName },
            };

            var response = await authenticatedRequestClient.Get(path, query, acceptHeader).ConfigureAwait(false);
            return ParseResponse<T>(response);
        }

        /// <summary>
        /// Retrieves and parses JSON from the specified endpoint.
        /// </summary>
        /// <typeparam name="T1">The type to parse the JSON into.</typeparam>
        /// <typeparam name="T2">The type to cast the JSON to after parsing.</typeparam>
        /// <param name="path">The API path.</param>
        /// <param name="query">An optional set of query parameters.</param>
        /// <returns>The parsed data.</returns>
        public async Task<T2> GetData<T1, T2>(string path, Dictionary<string, string> query = null) where T2 : class where T1 : T2
        {
            return await GetData<T1>(path, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Patches JSON from an API endpoint.
        /// </summary>
        /// <param name="path">The API Path.</param>
        /// <param name="query">The query parameters.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> Patch(string path, Dictionary<string, object> query = null)
        {
            using (var stringContent = new StringContent(JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json"))
            {
                var response = await authenticatedRequestClient.Patch(path, stringContent).ConfigureAwait(false);
                var responseBody = response.Content;

                // Since we _requested_ JSON, we are definitely not expecting to get an empty response
                // without hitting an exception.
                // Even an empty string is unexpected because that would not be valid JSON.
                if (string.IsNullOrEmpty(responseBody))
                {
                    throw new Float.Core.Net.HttpRequestException(response);
                }
            }

            return true;
        }

        /// <summary>
        /// Invokes an arbitray POST request on the API.
        /// </summary>
        /// <param name="path">The path to send the request to.</param>
        /// <param name="query">Optional additional query params.</param>
        /// <param name="requestBody">Optional request body.</param>
        /// <returns>The server response body.</returns>
        public async Task<string> Post(string path, Dictionary<string, string> query = null, string requestBody = null)
        {
            using var body = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await authenticatedRequestClient.Post(path, query, body: body).ConfigureAwait(false);
            return response.Content;
        }

        /// <summary>
        /// Invokes an arbitray request on the API.
        /// </summary>
        /// <param name="method">The method of the request.</param>
        /// <param name="path">The path to send the request to.</param>
        /// <param name="query">Optional additional query params.</param>
        /// <param name="requestBody">Optional request body.</param>
        /// <param name="contentType">Optional content type of the POST request.</param>
        /// <returns>The server raw response.</returns>
        public async Task<Response> RawResponse(string method, string path, Dictionary<string, string> query = null, string requestBody = null, string contentType = "application/json")
        {
            var httpMethod = new HttpMethod(method);
            using var body = string.IsNullOrEmpty(requestBody) ? null : new StringContent(requestBody, Encoding.UTF8, contentType);
            return await authenticatedRequestClient.Send(httpMethod, path, query, body: body).ConfigureAwait(false);
        }

        /// <summary>
        /// Invokes a DELETE request on the API.
        /// </summary>
        /// <param name="path">The path to send the request to.</param>
        /// <param name="query">Optional additional query params.</param>
        /// <returns>The server response.</returns>
        public async Task<string> Delete(string path, Dictionary<string, string> query = null)
        {
            var response = await authenticatedRequestClient.Send(HttpMethod.Delete, path, query).ConfigureAwait(false);
            return response.Content;
        }

        /// <summary>
        /// Loads more results for a paged response.
        /// </summary>
        /// <typeparam name="T">The type of data in each row of the response.</typeparam>
        /// <param name="pagedResponse">The original paged response.</param>
        /// <returns>The updated paged response.</returns>
        public async Task<IPagedResponse<T>> LoadMore<T>(IPagedResponse<T> pagedResponse)
        {
            if (pagedResponse == null)
            {
                throw new ArgumentNullException(nameof(pagedResponse));
            }

            if (pagedResponse.IsLastPage)
            {
                return pagedResponse;
            }

            var originalUri = pagedResponse.OriginalUri;
            var originalQuery = HttpUtility.ParseQueryString(originalUri.Query);
            var query = originalQuery
                .Keys
                .Cast<string>()
                .ToDictionary(k => k, v => originalQuery[v]);

            query["page"] = $"{pagedResponse.CurrentPage + 1}";

            return (IPagedResponse<T>)await GetData<PagedResponse<Node>>(originalUri.LocalPath, query).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> SendPushToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token cannot be null, empty, or whitespace.", nameof(token));
            }

            var requestJson = JsonConvert.SerializeObject(new { token, device = $"{DeviceInfo.Manufacturer} {DeviceInfo.Model}" });

            using (var body = new StringContent(requestJson, Encoding.UTF8, "application/json"))
            {
                _ = await authenticatedRequestClient.Post("api/push_notification_token", body).ConfigureAwait(false);
            }

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> SendDeletePushToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token cannot be null, empty, or whitespace.", nameof(token));
            }

            var requestJson = JsonConvert.SerializeObject(new { token });

            using (var body = new StringContent(requestJson, Encoding.UTF8, "application/json"))
            {
                _ = await authenticatedRequestClient.Send(HttpMethod.Delete, "api/push_notification_token", null, null, body).ConfigureAwait(false);
            }

            return true;
        }

        T ParseResponse<T>(Response response)
        {
            // Since we _requested_ JSON, we are definitely not expecting to get an empty response
            // without hitting an exception.
            // Even an empty string is unexpected because that would not be valid JSON.
            if (string.IsNullOrEmpty(response.Content))
            {
                throw new Float.Core.Net.HttpRequestException(response);
            }

            var result = Deserialize<T>(response.Content);

            if (result is PartialResponse pagedResponse)
            {
                pagedResponse.OriginalUri = response.HttpResponse.RequestMessage.RequestUri;
            }

            return result;
        }

        T Deserialize<T>(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new PrivateSetterContractResolver(),
                };

                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (JsonReaderException e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
            }

            return default;
        }
    }
}
