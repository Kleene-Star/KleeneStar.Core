using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Slas
{
    /// <summary>
    /// REST selection of the available <see cref="SlaPriority"/> values.
    /// </summary>
    [Title("SLA priority")]
    public sealed class Priority : RestApiSelection<SlaPolicy>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Priority()
        {
        }

        /// <summary>
        /// Returns one selection item per <see cref="SlaPriority"/> enum value, with the
        /// localized label looked up from <c>kleenestar.core:sla.priority.&lt;value&gt;.label</c>.
        /// </summary>
        /// <param name="query">The query criteria. Ignored — the selection is the fixed enum.</param>
        /// <param name="context">The query context. Ignored.</param>
        /// <param name="request">
        /// The request used to resolve the active culture for the label translation.
        /// </param>
        /// <returns>The selection items, one per enum value.</returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<SlaPolicy> query, IQueryContext context, IRequest request)
        {
            var list = Enum.GetValues<SlaPriority>()
                .Select(p => new RestApiSelectionItem
                {
                    Id = Guid.NewGuid(),
                    Text = I18N.Translate(request, $"kleenestar.core:sla.priority.{p.ToString().ToLowerInvariant()}.label")
                })
                .ToList();

            return list.AsQueryable();
        }

        /// <summary>
        /// Filtering is not meaningful for a fixed enum picker — the supplied query is
        /// returned unchanged.
        /// </summary>
        /// <param name="filter">The free-text filter expression. Ignored.</param>
        /// <param name="query">The policy query.</param>
        /// <param name="request">The request providing operational context.</param>
        /// <returns>The unchanged query.</returns>
        protected override IQuery<SlaPolicy> Filter(string filter, IQuery<SlaPolicy> query, IRequest request)
        {
            return query;
        }
    }
}
