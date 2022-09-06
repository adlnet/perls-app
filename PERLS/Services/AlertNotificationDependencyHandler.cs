using System;
using Float.Core.Analytics;
using Float.Core.Extensions;
using Float.Core.Net;
using Newtonsoft.Json;
using PERLS.Data;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Services
{
    /// <summary>
    /// Default notification handler.
    /// </summary>
    public class AlertNotificationDependencyHandler : Float.Core.Notifications.AlertNotificationHandler
    {
        /// <inheritdoc />
        public override void NotifyException(Exception e, string implication = null)
        {
            DependencyService.Get<AnalyticsService>().TrackException(e);
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Exception: {e}");
#endif
            base.NotifyException(e, implication);
        }

        /// <inheritdoc />
        public override string FormatException(Exception e)
        {
            if (e == null)
            {
                return null;
            }

            if (e is AggregateException aggregateException)
            {
                return FormatException(aggregateException.InnerException);
            }

            if (e.IsOfflineException())
            {
                return base.FormatException(e);
            }

            if (e is Float.FileDownloader.DownloadException downloadException)
            {
                if (e.InnerException != null)
                {
                    return FormatException(downloadException.InnerException);
                }

                return base.FormatException(downloadException);
            }

            return e switch
            {
                JsonException _ => Strings.JsonErrorMessage,
                HttpRequestException error when error.Code == 401 || error.Code == 403 => Strings.AuthenticationErrorMessage,
                HttpRequestException error when error.Code >= 500 => Strings.ServerErrorMessage,
                HttpRequestException error => error.Message,
                SetServerViewModelException error => error.Message,
                _ => Strings.UnknownErrorMessage,
            };
        }
    }
}
