using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing <see cref="Calendar"/> entities attached to a <see cref="Class"/>.
    /// </summary>
    public interface ICalendarManager : IComponentManager
    {
        /// <summary>
        /// Raised when a new calendar has been added.
        /// </summary>
        event EventHandler<Calendar> CalendarAdded;

        /// <summary>
        /// Raised when a calendar has been updated.
        /// </summary>
        event EventHandler<Calendar> CalendarUpdated;

        /// <summary>
        /// Raised when a calendar has been removed.
        /// </summary>
        event EventHandler<Calendar> CalendarRemoved;

        /// <summary>
        /// Returns the calendar identified by the supplied id, including its child collections.
        /// </summary>
        /// <param name="calendarId">The calendar id.</param>
        /// <returns>The calendar, or <c>null</c>.</returns>
        Calendar GetCalendar(Guid calendarId);

        /// <summary>
        /// Returns the calendar identified by the supplied parameter.
        /// </summary>
        /// <param name="calendarId">The id parameter.</param>
        /// <returns>The calendar, or <c>null</c>.</returns>
        Calendar GetCalendar(CalendarIdParameter calendarId);

        /// <summary>
        /// Returns all calendars attached to the specified class.
        /// </summary>
        /// <param name="classId">The class id parameter.</param>
        /// <returns>The calendars.</returns>
        IEnumerable<Calendar> GetCalendars(ClassIdParameter classId);

        /// <summary>
        /// Returns all calendars attached to the specified class.
        /// </summary>
        /// <param name="classId">The class id.</param>
        /// <returns>The calendars.</returns>
        IEnumerable<Calendar> GetCalendars(Guid classId);

        /// <summary>
        /// Returns calendars satisfying the supplied query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The matching calendars.</returns>
        IEnumerable<Calendar> GetCalendars(IQuery<Calendar> query);

        /// <summary>
        /// Returns calendars satisfying the supplied query in the supplied query context.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching calendars.</returns>
        IEnumerable<Calendar> GetCalendars(IQuery<Calendar> query, IQueryContext context);

        /// <summary>
        /// Adds a calendar to the manager.
        /// </summary>
        /// <param name="calendar">The calendar to add.</param>
        /// <returns>The current instance.</returns>
        ICalendarManager Add(Calendar calendar);

        /// <summary>
        /// Updates an existing calendar.
        /// </summary>
        /// <param name="calendar">The calendar to update.</param>
        /// <returns>The current instance.</returns>
        ICalendarManager Update(Calendar calendar);

        /// <summary>
        /// Removes the calendar identified by the supplied id.
        /// </summary>
        /// <param name="calendarId">The id of the calendar to remove.</param>
        /// <returns>The current instance.</returns>
        ICalendarManager Remove(Guid calendarId);
    }
}
