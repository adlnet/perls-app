using System;
using System.Threading.Tasks;
using Float.Core.Extensions;
using Float.Core.Notifications;
using Xamarin.Forms;

namespace PERLS
{
    /// <summary>
    /// Extensions on the main application class.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Open the provided URI with the given application class.
        /// </summary>
        /// <param name="application">The current application.</param>
        /// <param name="uri">The URI to open.</param>
        /// <returns>An awaitable task.</returns>
        public static Task<bool> OpenUriAsync(this Application application, Uri uri)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return ((App)application).OpenUri(uri);
        }

        /// <summary>
        /// Open the provided URI with the given application class, with automatic error reporting.
        /// </summary>
        /// <remarks>Use this when opening a URI from a synchronous method where you are not concerned with the result.</remarks>
        /// <param name="application">The current application.</param>
        /// <param name="uri">The URI to open.</param>
        public static void OpenUri(this Application application, Uri uri)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            application.OpenUriAsync(uri).OnFailure(task =>
            {
                DependencyService.Get<INotificationHandler>().NotifyException(task.Exception?.InnerException);
            });
        }

        /// <summary>
        /// Shorthand for getting a color from the application resources.
        /// </summary>
        /// <param name="application">The current application.</param>
        /// <param name="name">The name of the color to get.</param>
        /// <returns>The color from the application resources.</returns>
        public static Color Color(this Application application, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            return (Color)application.Resources[name];
        }
    }
}
