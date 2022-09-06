using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Analytics;
using Float.Core.Commands;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using PERLS.Data.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.Droid.Analytics
{
    /// <summary>
    /// Analytics tracking for App Center.
    /// </summary>
    public class AppCenterAnalytics : AnalyticsService, IAnalyticsService
    {
        ICommand trackPageCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCenterAnalytics"/> class.
        /// </summary>
        public AppCenterAnalytics() : base()
        {
            trackPageCommand = new DebounceCommand(TrackPage);

            Task.Run(async () =>
            {
                var memoryStatus = await Crashes.HasReceivedMemoryWarningInLastSessionAsync().ConfigureAwait(false);

                if (memoryStatus)
                {
                    TrackEvent("ReceivedMemoryWarningInLastSession", new Dictionary<string, string>
                    {
                        { "availableMemory", GC.GetTotalMemory(false).ToString(CultureInfo.InvariantCulture) },
                        { "model", DeviceInfo.Model },
                        { "manufacturer", DeviceInfo.Manufacturer },
                        { "osVersion", DeviceInfo.VersionString },
                    });
                }

                var crashStatus = await Crashes.HasCrashedInLastSessionAsync().ConfigureAwait(false);

                if (crashStatus)
                {
                    var crashReport = await Crashes.GetLastSessionCrashReportAsync().ConfigureAwait(false);

                    TrackEvent("CrashedInLastSession", new Dictionary<string, string>
                    {
                        { "stackTrace", crashReport.AndroidDetails.StackTrace },
                        { "threadName", crashReport.AndroidDetails.ThreadName },
                    });
                }
            });
        }

        /// <inheritdoc />
        public void SetUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            AppCenter.SetUserId(userId);
        }

        /// <inheritdoc />
        protected override void OnException(Exception exception)
        {
            Crashes.TrackError(exception);
        }

        /// <inheritdoc />
        protected override void OnPageView(string name, Page page)
        {
            trackPageCommand.Execute(name);
        }

        /// <inheritdoc />
        protected override void OnEvent(string eventName, Dictionary<string, string> parameters)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventName, parameters);
        }

        void TrackPage(object arg)
        {
            if (arg is string name)
            {
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("PageView", new Dictionary<string, string> { { "pageName", name } });
            }
        }
    }
}
