using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Analytics;
using Float.Core.Extensions;
using Float.Core.ViewModels;
using Newtonsoft.Json;
using PERLS;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ExperienceAPI;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <inheritdoc />
    public class NotificationNavigation : INotificationNavigation
    {
        /// <inheritdoc />
        public ICommand ItemSelected => DependencyService.Get<INavigationCommandProvider>().ItemSelected;

        /// <inheritdoc />
        public void NavigateByAction(string action, Dictionary<string, string> item = null, Dictionary<string, string> relatedItems = null)
        {
            DependencyService.Get<IReportingService>()
                            .ReportPushNotificationSelected(
                                ActivityBuilder
                                .FromNotification(action), action);

            var analytics = DependencyService.Get<AnalyticsService>();

            analytics.TrackEvent("Opened notification", new Dictionary<string, string>
            {
                { "action", action },
                { "item", JsonConvert.SerializeObject(item) },
            });

            Navigate(action, item, relatedItems)
                .OnFailure(task =>
                {
                    analytics.TrackException(task.Exception.InnerException);
                });
        }

        /// <summary>
        /// Asynchronously navigates to the view related to the received notification.
        /// </summary>
        /// <param name="action">The action from the notification.</param>
        /// <param name="item">The notification item.</param>
        /// <param name="relatedItems">Additional items related to the notification (not currently used).</param>
        /// <returns>An awaitable task.</returns>
        /// <remarks>
        /// Notification payloads are expected to be formatted something like this:
        ///
        ///     {
        ///         "action": "{action name}",
        ///         "image": "an image to associate with the notification",
        ///         "item": {
        ///             "type": "the type of item",
        ///             "uri": "a uri to the item; could be relative or absolute"
        ///         },
        ///         "related_items": [
        ///             {
        ///                 "type": "...",
        ///                 "uri": "..."
        ///             }
        ///         ]
        ///     }
        ///
        /// The `action` _should_ represent an action the application should take when handling the notification.
        /// The initial set of push notifications for the app treated it as a "category" which won't be appropriate moving forward.
        /// </remarks>
        async Task Navigate(string action, Dictionary<string, string> item = null, Dictionary<string, string> relatedItems = null)
        {
            switch (action)
            {
                case "goal":
                    await NavigateShell(ShellPaths.GoalsPath);
                    break;
                case "achievementEarned":
                    Uri url = GetItemUri(item);
                    string uuid = url.Segments.Last();
                    string type = GetItemData(item, "type");

                    // @todo: Ideally navigating to an achievement would be offloaded to primary navigation coordinator
                    switch (type)
                    {
                        case "badge":
                            var badge = await DependencyService.Get<ILearnerProvider>().GetBadgeItemFromId(uuid);

                            await NavigateShell(ShellPaths.BadgePath);
                            await NavigateToObject(new BadgeViewModel(badge));
                            break;
                        case "certificate":
                            var certificate = await DependencyService.Get<ILearnerProvider>().GetCertificateItemFromId(uuid);

                            await NavigateShell(ShellPaths.CertificatePath);
                            await NavigateToObject(new CertificateViewModel(certificate, ItemSelected));
                            break;
                        default:
                            throw new ArgumentException($"{type} is not a valid achievement type", "type");
                    }

                    break;
                case "NewPodcastEpisodeAdded":
                    await NavigateShell(ShellPaths.PodcastPath);
                    await NavigateToItem(item);
                    break;

                case "new_content":
                case "view_item":
                    await NavigateToItem(item);
                    break;

                default:
                    // Unrecognized action. Must be from a newer server version. We'll just do nothing.
                    break;
            }
        }

        /// <summary>
        /// Attempts to navigate the shell UI to the screen relevant to the notification.
        /// </summary>
        /// <param name="path">The shell path.</param>
        /// <remarks>
        /// Eventually, _all_ navigation should be handled by the <see cref="PERLS.Coordinators.PrimaryNavigationCoordinator"/>, including changing the current shell item.
        /// </remarks>
        Task NavigateShell(string path)
        {
            return Device.InvokeOnMainThreadAsync(() =>
            {
                return Shell.Current.GoToAsync(path);
            });
        }

        /// <summary>
        /// Attempts to resolve a URI for the navigation item and navigate to that item.
        /// </summary>
        /// <param name="item">The navigation item.</param>
        /// <remarks>
        /// Navigation is passed off to <see cref="App"/> (where it can be handled by the <see cref="PERLS.Coordinators.PrimaryNavigationCoordinator"/>.
        /// </remarks>
        Task NavigateToItem(Dictionary<string, string> item)
        {
            return Device.InvokeOnMainThreadAsync(() =>
            {
                var uri = GetItemUri(item);
                return Application.Current.OpenUriAsync(uri);
            });
        }

        /// <summary>
        /// Attempts to navigate to a view that can display the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model to show to the user.</param>
        /// <remarks>
        /// Using URI navigation is preferred over navigating by view model.
        /// </remarks>
        Task NavigateToObject(BaseViewModel viewModel)
        {
            return Device.InvokeOnMainThreadAsync(() =>
            {
                ItemSelected?.Execute(viewModel);
            });
        }

        /// <summary>
        /// Retrieves the URI for the notification item.
        /// </summary>
        /// <param name="item">The notification item.</param>
        /// <returns>An absolute URI to the notification item.</returns>
        /// <exception cref="UriFormatException">Thrown when the item URI is not valid (this is likely a server issue).</exception>
        /// <exception cref="ArgumentNullException">Thrown when there was no notification item.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the notification item didn't have a URI.</exception>
        Uri GetItemUri(Dictionary<string, string> item)
        {
            var url = GetItemData(item, "url");
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri)
            {
                return new Uri(DependencyService.Get<IAppContextService>().Server, uri);
            }

            return uri;
        }

        /// <summary>
        /// Retrieves a specific data value from the notification item.
        /// </summary>
        /// <param name="item">The notification item.</param>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when there was no notification item.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the notification item exists, but the key was not present.</exception>
        string GetItemData(Dictionary<string, string> item, string key)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return item[key];
        }
    }
}
