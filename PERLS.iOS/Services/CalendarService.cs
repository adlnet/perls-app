using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventKit;
using EventKitUI;
using Float.Core.Analytics;
using Foundation;
using PERLS.Data.Definition;
using PERLS.Services;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace PERLS.iOS.Services
{
    /// <summary>
    /// The iOS implementation of the Calendar Service.
    /// </summary>
    public class CalendarService : EKEventEditViewDelegate, ICalendarService
    {
        readonly EKEventStore eventStore = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarService"/> class.
        /// </summary>
        public CalendarService()
        {
        }

        /// <summary>
        /// Gets the top view controller.
        /// </summary>
        /// <value>
        /// The top view controller.
        /// </value>
        UIViewController TopViewController
        {
            get
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }

                return vc;
            }
        }

        /// <inheritdoc/>
        public override void Completed(EKEventEditViewController controller, EKEventEditViewAction action)
        {
            TopViewController.DismissViewController(true, null);
        }

        /// <summary>
        /// Disposes the objects.
        /// </summary>
        public new void Dispose()
        {
            eventStore.Dispose();
            base.Dispose();
        }

        /// <inheritdoc/>
        public void ScheduleEvent(ICalendarEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                return;
            }

            eventStore.RequestAccess(
                EKEntityType.Event,
                (bool granted, NSError e) =>
                {
                    if (!granted)
                    {
                        // We could show an alert, but acutally if we still just show the modal is takes care of it for us.
                        // In the meantime we can log it to analytics.
                        DependencyService.Get<AnalyticsService>().TrackEvent("Calendar Access Denied", new Dictionary<string, string> { { "exceptionMessage", e.DebugDescription } });
                        return;
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        using var controller = new EKEventEditViewController
                        {
                            EditViewDelegate = this,
                            EventStore = eventStore,
                        };

                        using var eventItem = EKEvent.FromStore(controller.EventStore);
                        eventItem.StartDate = NSDate.FromTimeIntervalSince1970(calendarEvent.StartDateOffset.ToUnixTimeSeconds());
                        eventItem.EndDate = NSDate.FromTimeIntervalSince1970(calendarEvent.EndDateOffset.ToUnixTimeSeconds());
                        eventItem.Location = calendarEvent.Location;
                        eventItem.Title = calendarEvent.Title;
                        controller.Event = eventItem;

                        if (calendarEvent.HasRecurrence)
                        {
                            EKRecurrenceEnd recurrenceEnd = null;

                            if (calendarEvent.RecurrenceCount != -1)
                            {
                                recurrenceEnd = EKRecurrenceEnd.FromOccurrenceCount(Convert.ToInt32(calendarEvent.RecurrenceCount, CultureInfo.CurrentCulture));
                            }
                            else if (calendarEvent.RecurrenceUntil != default)
                            {
                                recurrenceEnd = EKRecurrenceEnd.FromEndDate(calendarEvent.RecurrenceUntil.Date.ToNSDate());
                            }

                            // Unfortunately Apple wants to use structs for everything here... So it gets to be very fun.
                            var recurrenceDays = calendarEvent.RecurrenceDays.Select((arg) => arg switch
                            {
                                "SU" => EKRecurrenceDayOfWeek.FromDay(EKDay.Sunday),
                                "MO" => EKRecurrenceDayOfWeek.FromDay(EKDay.Monday),
                                "TU" => EKRecurrenceDayOfWeek.FromDay(EKDay.Tuesday),
                                "WE" => EKRecurrenceDayOfWeek.FromDay(EKDay.Wednesday),
                                "TH" => EKRecurrenceDayOfWeek.FromDay(EKDay.Thursday),
                                "FR" => EKRecurrenceDayOfWeek.FromDay(EKDay.Friday),
                                "SA" => EKRecurrenceDayOfWeek.FromDay(EKDay.Saturday),
                                _ => EKRecurrenceDayOfWeek.FromDay(EKDay.Saturday),
                            });

                            using var recurrenceRule = new EKRecurrenceRule(EKRecurrenceFrequency.Daily, calendarEvent.RecurrenceInterval, recurrenceDays.ToArray(), null, null, null, null, null, recurrenceEnd);
                            eventItem.AddRecurrenceRule(recurrenceRule);
                        }

                        TopViewController.PresentViewController(controller, true, null);
                    });
                });
        }
    }
}
