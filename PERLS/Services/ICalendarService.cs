using PERLS.Data.Definition;

namespace PERLS.Services
{
    /// <summary>
    /// The Calendar service.
    /// </summary>
    public interface ICalendarService
    {
        /// <summary>
        /// Schedules an event in the user's calendar app.
        /// </summary>
        /// <param name="eventItem">The event item.</param>
        void ScheduleEvent(ICalendarEvent eventItem);
    }
}
