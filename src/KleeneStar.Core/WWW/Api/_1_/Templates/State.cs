using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Templates
{
    /// <summary>
    /// Represents a selectable state for use in REST API selection scenarios.
    /// </summary>
    [Title("Template state")]
    public sealed class State : RestApiSelection<Model.Entities.Template>
    {
        /// <summary>
        /// Retrieves a queryable collection of index items that match the specified query criteria.
        /// </summary>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Model.Entities.Template> query, IQueryContext context, IRequest request)
        {
            var list = new List<RestApiSelectionItem>()
            {
                new()
                {
                    Id = TemplateState.Active.Id(),
                    Text = I18N.Translate(request, TemplateState.Active.Text()),
                    Color = TemplateState.Active.Color()
                },
                new()
                {
                    Id = TemplateState.Archived.Id(),
                    Text = I18N.Translate(request, TemplateState.Archived.Text()),
                    Color = TemplateState.Archived.Color()
                }
            };

            return list.AsQueryable();
        }
    }
}
