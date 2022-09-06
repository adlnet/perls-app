using System;
using System.Collections.Generic;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The interface for the calendar event.
    /// </summary>
    public interface ICalendarEvent
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        string Location { get; }

        /// <summary>
        /// Gets the start date offset.
        /// </summary>
        /// <value>
        /// The start date offset.
        /// </value>
        DateTimeOffset StartDateOffset { get; }

        /// <summary>
        /// Gets the end date offset.
        /// </summary>
        /// <value>
        /// The end date offset.
        /// </value>
        DateTimeOffset EndDateOffset { get; }

        /// <summary>
        /// Gets the recurrence frequency.
        /// </summary>
        /// <value>
        /// The recurrence frequency.
        /// </value>
        RecurrenceFrequency RecurrenceFrequency { get; }

        /// <summary>
        /// Gets the recurrence interval.
        /// </summary>
        /// <value>
        /// The recurrence interval.
        /// </value>
        int RecurrenceInterval { get; }

        /// <summary>
        /// Gets the recurrence until date.
        /// </summary>
        /// <value>
        /// The recurance until date.
        /// </value>
        DateTimeOffset RecurrenceUntil { get; }

        /// <summary>
        /// Gets the recurrence count.
        /// </summary>
        /// <value>
        /// The recurrence count.
        /// </value>
        int RecurrenceCount { get; }

        /// <summary>
        /// Gets the recurrence days of the week.
        /// </summary>
        /// <value>
        /// The recurrence days.
        /// </value>
        IEnumerable<string> RecurrenceDays { get; }

        /// <summary>
        /// Gets a value indicating whether or not this calendar event has recurrence.
        /// </summary>
        /// <value>
        /// A value indicating whether or not this calendar event has recurrence.
        /// </value>
        bool HasRecurrence { get; }
    }
}
