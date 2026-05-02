using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Templates._workspacekey_
{
    /// <summary>
    /// Provides CRUD operations for template items via a REST API.
    /// </summary>
    [Cache]
    public sealed class Index : RestApiCrud<Model.Entities.Template>
    {
        /// <summary>
        /// Creates a new instance of an object that implements the IQueryContext interface.
        /// </summary>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves a queryable collection of templates that match the specified query criteria.
        /// </summary>
        protected override IEnumerable<Model.Entities.Template> Retrieve(IQuery<Model.Entities.Template> query, IQueryContext context, IRequest request)
        {
            return CoreHub.TemplateManager.GetTemplates(query, context);
        }
    }
}
