using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPermission;
using KleeneStar.Core.WebRestApi;
using System;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Calendar._calendarid_
{
    /// <summary>
    /// Serves the permission dialog of a calendar: which group holds which policy on it.
    /// </summary>
    [IncludeSubPaths]
    [Cache]
    public sealed class Permission : RestApiPermissionScoped
    {
        /// <summary>
        /// Gets the kind of resource this endpoint administers.
        /// </summary>
        protected override string Scope => PermissionScope.Calendar;

        /// <summary>
        /// Returns the calendar the request addresses.
        /// </summary>
        /// <param name="request">The request whose route names the calendar.</param>
        /// <returns>The calendar id, or null when the route addresses none.</returns>
        protected override string ResolveScopeId(IRequest request)
        {
            var id = request?.GetParameter<CalendarIdParameter>()?.Value;

            return Guid.TryParse(id, out var calendarId)
                ? CoreHub.CalendarManager.GetCalendar(calendarId)?.Id.ToString()
                : null;
        }
    }
}
