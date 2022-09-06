using System;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Provider;
using PERLS.Data.Definition;
using PERLS.Services;
using static Android.Provider.CalendarContract;

namespace PERLS.Droid
{
    /// <summary>
    /// The Android Calendar Service.
    /// </summary>
    public class CalendarService : ICalendarService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarService"/> class.
        /// </summary>
        public CalendarService()
        {
        }

        /// <inheritdoc/>
        public void ScheduleEvent(ICalendarEvent eventItem)
        {
            if (eventItem == null)
            {
                return;
            }

            var beginTimeInMillis = GetTimeInMillis(eventItem.StartDateOffset);
            var endTimeInMillis = GetTimeInMillis(eventItem.EndDateOffset);

            using var intent = new Intent(Intent.ActionEdit);
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            intent.SetData(Events.ContentUri);

            // As dumb as this may seem, ignoring this is necessary because the build server errors out if we use the new item.
#pragma warning disable CS0618 // Type or member is obsolete
            intent.PutExtra(EventsColumns.Title, eventItem.Title);
            intent.PutExtra(EventsColumns.EventLocation, eventItem.Location);
            intent.PutExtra(ExtraEventBeginTime, beginTimeInMillis);
            intent.PutExtra(ExtraEventEndTime, endTimeInMillis);

            if (eventItem.HasRecurrence)
            {
                var rrule = $"FREQ={eventItem.RecurrenceFrequency};";

                if (eventItem.RecurrenceCount != -1)
                {
                    rrule += $"INTERVAL={eventItem.RecurrenceInterval};";
                }

                if (eventItem.RecurrenceCount != -1)
                {
                    rrule += $"COUNT={eventItem.RecurrenceCount};";
                }

                if (eventItem.RecurrenceUntil != default)
                {
                    rrule += $"UNTIL={eventItem.RecurrenceUntil.ToString("yyyyMMdd", CultureInfo.CurrentCulture)};";
                }

                rrule += $"BYDAY={string.Join(",", eventItem.RecurrenceDays)};";
                intent.PutExtra(EventsColumns.Rrule, rrule);
#pragma warning restore CS0618

            }

            Android.App.Application.Context.StartActivity(intent);
        }

        long GetTimeInMillis(DateTimeOffset dateTimeOffset)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                var calendar = Android.Icu.Util.Calendar.Instance;

                // Months start on zero...
                calendar.Set(dateTimeOffset.Year, dateTimeOffset.Month - 1, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute);
                return calendar.TimeInMillis;
            }
            else
            {
                var calendar = Java.Util.Calendar.GetInstance(Java.Util.Locale.Default);

                // Months start on zero...
                calendar.Set(dateTimeOffset.Year, dateTimeOffset.Month - 1, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute);
                return calendar.TimeInMillis;
            }
        }
    }
}
