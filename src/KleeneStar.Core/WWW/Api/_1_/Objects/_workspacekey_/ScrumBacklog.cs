using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_
{
    /// <summary>
    /// REST API scrum backlog endpoint for the objects of a workspace.
    /// </summary>
    /// <remarks>
    /// Returns the product backlog. Until a dedicated Sprint entity exists in the model
    /// layer, this endpoint reports an empty set of sprints and items so the UI renders an
    /// empty backlog instead of failing.
    /// </remarks>
    [Title("kleenestar.core:object.view.scrum.backlog.title")]
    [Cache]
    public sealed class ScrumBacklog : RestApiScrumBacklog<Model.Entities.Object, Model.Entities.Object>
    {
        /// <inheritdoc />
        protected override IEnumerable<Model.Entities.Object> RetrieveSprints(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            return [];
        }

        /// <inheritdoc />
        protected override IEnumerable<Model.Entities.Object> RetrieveItems(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            return [];
        }
    }
}
