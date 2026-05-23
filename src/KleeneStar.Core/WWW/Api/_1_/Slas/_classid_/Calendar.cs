using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Slas._classid_
{
    using Calendar = KleeneStar.Model.Entities.Calendar;

    /// <summary>
    /// REST selection of the <see cref="Calendar"/> entries that belong to the class
    /// addressed by the URL <c>classid</c> segment. Powers the calendar dropdown on
    /// the SLA Add/Edit/Clone forms.
    /// </summary>
    /// <remarks>
    /// The <c>{classid}</c> URL segment is contributed by the parent <c>_classid_</c>
    /// folder — do NOT add <c>[ClassIdSegment]</c> here, or WebExpress would append a
    /// second variable segment and the literal "calendar" segment would be lost.
    /// </remarks>
    [Title("SLA calendar")]
    [Cache]
    public sealed class Calendar : RestApiSelection<SlaPolicy>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Calendar()
        {
        }

        /// <summary>
        /// Returns one selection item per <see cref="KleeneStar.Model.Entities.Calendar"/>
        /// that is active and belongs to the class addressed by the URL <c>{classid}</c>
        /// segment. The result is materialized inside the same <c>using</c> block as the
        /// owning <see cref="ModelHub.CreateDbContext"/> to avoid a deferred query bound
        /// to a disposed DbContext when the framework later enumerates the list.
        /// </summary>
        /// <param name="query">The query criteria. Ignored — the selection is class-scoped.</param>
        /// <param name="context">The query context. Ignored.</param>
        /// <param name="request">
        /// The request whose <c>{classid}</c> path segment drives the calendar lookup.
        /// </param>
        /// <returns>The materialized selection items, one per active calendar of the class.</returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<SlaPolicy> query, IQueryContext context, IRequest request)
        {
            var classIdParam = request.GetParameter<ClassIdParameter>();
            var classId = Guid.TryParse(classIdParam?.Value, out var id) ? id : Guid.Empty;

            var calendarQuery = new Query<KleeneStar.Model.Entities.Calendar>()
                .WhereEquals(x => x.ClassId, classId)
                .Where(x => x.State == CalendarState.Active);

            // materialize INSIDE the using-block. RestApiSelection<T> later iterates the
            // returned IQueryable (e.g. via Count() during paging) AFTER this method
            // returns — if we returned a deferred query bound to the disposed DbContext,
            // EF Core would throw ObjectDisposedException.
            List<RestApiSelectionItem> items;
            using (var db = ModelHub.CreateDbContext())
            {
                items = [.. CoreHub.CalendarManager
                    .GetCalendars(calendarQuery, db)
                    .Select(c => new RestApiSelectionItem
                    {
                        Id = c.Id,
                        Text = c.Name
                    })];
            }

            return items.AsQueryable();
        }

        /// <summary>
        /// Filtering is not meaningful for a small class-scoped calendar picker — the
        /// supplied query is returned unchanged.
        /// </summary>
        /// <param name="filter">The free-text filter expression. Ignored.</param>
        /// <param name="query">The query.</param>
        /// <param name="request">The request providing operational context.</param>
        /// <returns>The unchanged query.</returns>
        protected override IQuery<SlaPolicy> Filter(string filter, IQuery<SlaPolicy> query, IRequest request)
        {
            return query;
        }
    }
}
