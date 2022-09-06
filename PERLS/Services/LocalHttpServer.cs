using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using Float.Core.Analytics;
using Float.HttpServer;
using Float.TinCan.ActivityLibrary;
using PERLS.Services.HttpServerResponder;
using Xamarin.Forms;
using static Float.HttpServer.HttpRouter;
using HttpServer = Float.HttpServer.LocalHttpServer;

namespace PERLS.Services
{
    /// <summary>
    /// A local server to serve local files.
    /// </summary>
    public class LocalHttpServer : INotifyPropertyChanged
    {
        const string ServerScheme = "http";
        readonly HttpServer server;
        Uri uri;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalHttpServer"/> class.
        /// </summary>
        public LocalHttpServer()
        {
            var host = "127.0.0.1";
            var address = $"{ServerScheme}://{host}";
            var port = PortSelector.SelectForAddress(address, 62550);
            server = new HttpServer(host, port.SelectedPort);
            UpdateUri();
            var documentRoot = FileStorage.PackagedContentDirectory;
            server.SetDefaultResponder(new StaticFileResponder(documentRoot));
            server.SetErrorResponder(new ServerFallbackErrorResponder404());
            server.Use(
                new List<HttpMethod> { HttpMethod.GET, HttpMethod.POST, HttpMethod.PATCH },
                (HttpListenerRequest request, ref HttpListenerResponse response) =>
                {
                    var allowed = DependencyService.Get<HttpServerSecurity>().Lookup(request.Cookies);
                    if (!allowed)
                    {
                        response.StatusCode = 401;
                    }

                    return allowed;
                });
            server.Use(
                new List<HttpMethod> { HttpMethod.GET },
                (HttpListenerRequest request, ref HttpListenerResponse response) =>
                {
                    if (!string.IsNullOrEmpty(request.Url.Query) && request.Url.AbsolutePath.Contains("/node"))
                    {
                        request.Headers.Add(StaticFileResponder.ExtendedFilePathCheckHeaderKey, "true");
                    }

                    return true;
                });
            server.Post("/node/:nodeId", new FeedbackFormActionHandler());

            DependencyService.Get<AnalyticsService>().TrackEvent("port_selected", new Dictionary<string, string>
            {
                ["selected_port"] = $"{port.SelectedPort}",
                ["rejected_port"] = string.Join(",", port.RejectedPorts),
            });
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the host name.
        /// </summary>
        /// <value>
        /// The host name for the http server.
        /// </value>
        public string Host => server.Host;

        /// <summary>
        /// Gets the port number.
        /// </summary>
        /// <value>
        /// The port number for the http server.
        /// </value>
        public ushort Port => server.Port;

        /// <summary>
        /// Gets the Uri of the server.
        /// </summary>
        /// <value>
        /// The Uri of the http server.
        /// </value>
        public Uri Uri
        {
            get => uri;
            private set
            {
                var shouldNotifyChange = uri?.OriginalString != value?.OriginalString;
                uri = value;
                if (shouldNotifyChange)
                {
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Stops the default server.
        /// </summary>
        public void Stop()
        {
            server.Stop();
        }

        /// <summary>
        /// Starts the default server. Note: This reselects the port.
        /// </summary>
        public void Start()
        {
            var address = $"{ServerScheme}://{Host}";
            var port = PortSelector.SelectForAddress(address, Port);
            server.Restart(port.SelectedPort);
            UpdateUri();
        }

        /// <summary>
        /// Checks status of server.
        /// </summary>
        /// <returns><c>True</c> if server is running.</returns>
        public bool IsServerAvailable()
        {
            return server.IsServerAvailable();
        }

        void UpdateUri()
        {
            var address = $"{ServerScheme}://{server.Host}";
            Uri = new Uri($"{address}:{server.Port}/");
        }

        /// <summary>
        /// Create the OnPropertyChanged method to raise the event.
        /// </summary>
        /// <param name="propertyName">The property name that is changing.</param>
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
