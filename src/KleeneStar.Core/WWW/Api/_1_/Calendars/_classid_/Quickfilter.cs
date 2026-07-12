using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Calendars._classid_
{
    using Calendar = KleeneStar.Model.Entities.Calendar;

    /// <summary>
    /// Quick-filter selection for calendar rows.
    /// </summary>
    public sealed class Quickfilter : RestApiQuickfilter<Calendar>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Quickfilter()
        {
        }

        /// <summary>
        /// Returns the quick-filter chips displayed above the calendar table. Selecting
        /// a chip narrows the table to one of the matching <see cref="CalendarState"/>
        /// values or to the calendar marked as <c>IsDefault</c>.
        /// </summary>
        /// <param name="context">The query context. Ignored — the chips are fixed.</param>
        /// <param name="request">The HTTP request providing operational context.</param>
        /// <returns>The fixed set of quick-filter items.</returns>
        protected override IEnumerable<RestApiQuickfilterItem> RetrieveItems(IQueryContext context, IRequest request)
        {
            yield return new RestApiQuickfilterItem { Id = "qf_active",   Name = "Active" };
            yield return new RestApiQuickfilterItem { Id = "qf_archived", Name = "Archived" };
            yield return new RestApiQuickfilterItem { Id = "qf_default",  Name = "Default" };
        }
    }
}
