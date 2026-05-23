using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Calendars
{
    using Calendar = KleeneStar.Model.Entities.Calendar;

    /// <summary>
    /// REST selection of common IANA timezone identifiers offered when creating or editing
    /// a <see cref="Calendar"/>. A free-form text field is allowed too; this selection is
    /// a convenience for the most common values.
    /// </summary>
    [Title("Calendar timezone")]
    [Cache]
    public sealed class TimeZone : RestApiSelection<Calendar>
    {
        private static readonly string[] _commonZones =
        [
            "UTC",
            "Europe/Berlin",
            "Europe/Vienna",
            "Europe/Zurich",
            "Europe/London",
            "Europe/Paris",
            "America/New_York",
            "America/Chicago",
            "America/Los_Angeles",
            "Asia/Tokyo",
            "Asia/Singapore",
            "Australia/Sydney"
        ];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TimeZone()
        {
        }

        /// <summary>
        /// Returns the fixed list of common IANA time-zone identifiers as selection items.
        /// </summary>
        /// <param name="query">The query criteria. Ignored — the selection is a fixed list.</param>
        /// <param name="context">The query context. Ignored.</param>
        /// <param name="request">The request providing operational context.</param>
        /// <returns>The list of time-zone selection items.</returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Calendar> query, IQueryContext context, IRequest request)
        {
            return _commonZones
                .Select(z => new RestApiSelectionItem
                {
                    Id = Guid.NewGuid(),
                    Text = z
                })
                .AsQueryable();
        }

        /// <summary>
        /// Filtering is not meaningful for a free-form time-zone picker — the supplied
        /// query is returned unchanged.
        /// </summary>
        /// <param name="filter">The free-text filter expression. Ignored.</param>
        /// <param name="query">The calendar query.</param>
        /// <param name="request">The request providing operational context.</param>
        /// <returns>The unchanged query.</returns>
        protected override IQuery<Calendar> Filter(string filter, IQuery<Calendar> query, IRequest request)
        {
            return query;
        }
    }
}
