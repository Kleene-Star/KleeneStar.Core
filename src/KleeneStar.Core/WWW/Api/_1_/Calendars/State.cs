using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Calendars
{
    using Calendar = KleeneStar.Model.Entities.Calendar;

    /// <summary>
    /// REST selection of the available <see cref="CalendarState"/> values.
    /// </summary>
    [Title("Calendar state")]
    public sealed class State : RestApiSelection<Calendar>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public State()
        {
        }

        /// <summary>
        /// Returns the two selectable <see cref="CalendarState"/> entries (Active, Archived),
        /// each tagged with its localized label and color so the picker can render them.
        /// </summary>
        /// <param name="query">The query criteria. Ignored — the selection is a fixed list.</param>
        /// <param name="context">The query context. Ignored.</param>
        /// <param name="request">
        /// The request used to resolve the active culture for the label translation.
        /// </param>
        /// <returns>The two-element list of selection items.</returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Calendar> query, IQueryContext context, IRequest request)
        {
            var list = new List<RestApiSelectionItem>
            {
                new()
                {
                    Id = CalendarState.Active.Id(),
                    Text = I18N.Translate(request, CalendarState.Active.Text()),
                    Color = CalendarState.Active.Color()
                },
                new()
                {
                    Id = CalendarState.Archived.Id(),
                    Text = I18N.Translate(request, CalendarState.Archived.Text()),
                    Color = CalendarState.Archived.Color()
                }
            };

            return list.AsQueryable();
        }

        /// <summary>
        /// Narrows the calendar query by name when a free-text filter is supplied
        /// (case-insensitive contains match). Returns the query unchanged when the
        /// filter is null or the literal string <c>"null"</c>.
        /// </summary>
        /// <param name="filter">The free-text filter expression.</param>
        /// <param name="query">The calendar query to refine.</param>
        /// <param name="request">The request providing operational context.</param>
        /// <returns>The (possibly refined) calendar query.</returns>
        protected override IQuery<Calendar> Filter(string filter, IQuery<Calendar> query, IRequest request)
        {
            if (filter is null || filter == "null")
            {
                return query;
            }

            return query.WhereContainsIgnoreCase(x => x.Name, filter);
        }
    }
}
