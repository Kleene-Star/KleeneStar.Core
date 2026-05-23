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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
