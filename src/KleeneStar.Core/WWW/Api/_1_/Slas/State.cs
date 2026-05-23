using KleeneStar.Model.Entities;
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
    /// REST selection of the available <see cref="SlaPolicyState"/> values.
    /// </summary>
    [Title("SLA state")]
    public sealed class State : RestApiSelection<SlaPolicy>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public State()
        {
        }

        /// <summary>
        /// Returns the four selectable <see cref="SlaPolicyState"/> entries (Draft,
        /// Active, Inactive, Archived), each tagged with its localized label and color
        /// so the picker can render them.
        /// </summary>
        /// <param name="query">The query criteria. Ignored — the selection is a fixed list.</param>
        /// <param name="context">The query context. Ignored.</param>
        /// <param name="request">
        /// The request used to resolve the active culture for the label translation.
        /// </param>
        /// <returns>The four-element list of selection items.</returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<SlaPolicy> query, IQueryContext context, IRequest request)
        {
            var list = new List<RestApiSelectionItem>
            {
                new()
                {
                    Id = SlaPolicyState.Draft.Id(),
                    Text = I18N.Translate(request, SlaPolicyState.Draft.Text()),
                    Color = SlaPolicyState.Draft.Color()
                },
                new()
                {
                    Id = SlaPolicyState.Active.Id(),
                    Text = I18N.Translate(request, SlaPolicyState.Active.Text()),
                    Color = SlaPolicyState.Active.Color()
                },
                new()
                {
                    Id = SlaPolicyState.Inactive.Id(),
                    Text = I18N.Translate(request, SlaPolicyState.Inactive.Text()),
                    Color = SlaPolicyState.Inactive.Color()
                },
                new()
                {
                    Id = SlaPolicyState.Archived.Id(),
                    Text = I18N.Translate(request, SlaPolicyState.Archived.Text()),
                    Color = SlaPolicyState.Archived.Color()
                }
            };

            return list.AsQueryable();
        }

        /// <summary>
        /// Narrows the SLA-policy query by name when a free-text filter is supplied
        /// (case-insensitive contains match). Returns the query unchanged when the
        /// filter is null or the literal string <c>"null"</c>.
        /// </summary>
        /// <param name="filter">The free-text filter expression.</param>
        /// <param name="query">The policy query to refine.</param>
        /// <param name="request">The request providing operational context.</param>
        /// <returns>The (possibly refined) policy query.</returns>
        protected override IQuery<SlaPolicy> Filter(string filter, IQuery<SlaPolicy> query, IRequest request)
        {
            if (filter is null || filter == "null")
            {
                return query;
            }

            return query.WhereContainsIgnoreCase(x => x.Name, filter);
        }
    }
}
