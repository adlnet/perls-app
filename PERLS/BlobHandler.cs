using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.DataImplementation.Models;
using PERLS.Services;
using Xamarin.Forms;

namespace PERLS
{
    /// <summary>
    /// The blob handler.
    /// </summary>
    public class BlobHandler : IJavaScriptActionHandler
    {
        /// <inheritdoc />
        public string AttachedJavaScript => @"
            (function() {
                var dateButtons = document.getElementsByClassName('daterecurical-btn');
                for (var i = 0; i < dateButtons.length; i++) {
                    var button = dateButtons[i];
                    button.onclick = (b) => { var event = drupalSettings.date_recur_ical;
                        let btnid = b.target.dataset.daterecurical;
                        var recurrence = '';
                        if (event[btnid].rrule.freq !== '') {
                            var recurrence = {
                                freq: event[btnid].rrule.freq,
                                interval: event[btnid].rrule.interval,
                                count: event[btnid].rrule.count,
                                until: event[btnid].rrule.until,
                                byday: event[btnid].byday
                            };
                        }

                        var data = {
                            action: ""blob_catcher"",
                            data: { title: event[btnid].title, description: event[btnid].description, location: event[btnid].location, start: event[btnid].start, end: event[btnid].end, recurrence: recurrence }
                        };

                        invokeCSharpWebFormSubmit(JSON.stringify(data));
                        b.stopImmediatePropagation();
                        b.preventDefault();
                    }
                }
            })();";

        /// <inheritdoc />
        public string ActionName => "blob_catcher";

        /// <inheritdoc />
        public void PerformAction(Effect element, object data)
        {
            if (data is not JObject dataObject)
            {
                return;
            }

            var title = dataObject.GetValue("title").Value<string>();
            var description = dataObject.GetValue("description").Value<string>();
            var location = dataObject.GetValue("location").Value<string>();
            var start = dataObject.GetValue("start").Value<string>();
            var end = dataObject.GetValue("end").Value<string>();
            var recurrence = dataObject.GetValue("recurrence") as JObject;
            var recurrenceInterval = recurrence != null && recurrence.TryGetValue("interval", out JToken interval) ? interval.Value<int>() : -1;
            var recurrenceFreq = recurrence != null && recurrence.TryGetValue("freq", out JToken freq) ? (RecurrenceFrequency)Enum.Parse(typeof(RecurrenceFrequency), freq.Value<string>(), true) : RecurrenceFrequency.NeverOrUnknown;
            var recurrenceCount = recurrence != null && recurrence.TryGetValue("count", out var count) ? count.Value<int>() : -1;
            var recurrenceDays = recurrence != null && recurrence.TryGetValue("byday", out var byDayArray) ? byDayArray.ToObject<IEnumerable<string>>() : Enumerable.Empty<string>();
            var recurrenceUntil = recurrence != null && recurrence.TryGetValue("until", out var until) ? until.Value<string>() : string.Empty;

            var calendarEvent = new CalendarEvent
            {
                Title = title,
                Description = description,
                Location = location,
                StartDateOffset = DateTimeOffset.Parse(start),
                EndDateOffset = DateTimeOffset.Parse(end),
                RecurrenceInterval = recurrenceInterval,
                RecurrenceFrequency = recurrenceFreq,
                RecurrenceCount = recurrenceCount,
                RecurrenceDays = recurrenceDays,
                RecurrenceUntil = string.IsNullOrEmpty(recurrenceUntil) ? default : DateTimeOffset.Parse(recurrenceUntil),
            };

            DependencyService.Get<ICalendarService>().ScheduleEvent(calendarEvent);
        }
    }
}
