using System;
using System.Collections.Generic;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A Calendar Event.
    /// </summary>
    public class CalendarEvent : ICalendarEvent
    {
        /// <inheritdoc/>
        public string Title { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public string Location { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset StartDateOffset { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset EndDateOffset { get; set; }

        /// <inheritdoc/>
        public RecurrenceFrequency RecurrenceFrequency { get; set; }

        /// <inheritdoc/>
        public int RecurrenceInterval { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset RecurrenceUntil { get; set; }

        /// <inheritdoc/>
        public int RecurrenceCount { get; set; }

        /// <inheritdoc/>
        public IEnumerable<string> RecurrenceDays { get; set; }

        /// <inheritdoc/>
        public bool HasRecurrence => RecurrenceFrequency != RecurrenceFrequency.NeverOrUnknown;
    }
}
