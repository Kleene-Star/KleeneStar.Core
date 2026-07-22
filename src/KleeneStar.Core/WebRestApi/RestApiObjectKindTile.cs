using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Core.WebRestApi
{
    /// <summary>
    /// Project-wide base for the object tile endpoint of a kind's overview tab control:
    /// the workspace's objects of the <see cref="Kind"/> as a card grid. A concrete
    /// subclass only fixes the kind it lists (issue, asset, …); each concrete endpoint
    /// registers at its own route, so the base must stay abstract.
    /// </summary>
    public abstract class RestApiObjectKindTile : RestApiTile<Model.Entities.Object>
    {
        /// <summary>
        /// Gets the persisted kind key the tile view is scoped to.
        /// </summary>
        protected abstract string Kind { get; }

        /// <summary>
        /// Creates a new instance of an object that implements the IQueryContext interface.
        /// </summary>
        /// <returns>An IQueryContext instance that can be used to execute queries.</returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves the tile items: the workspace's objects of the endpoint's kind.
        /// </summary>
        /// <param name="query">The query (carries the applied search filter and paging).</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The tile items, each opening its object detail page when selected.</returns>
        protected override IEnumerable<RestApiTileItem> RetrieveItems(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            var key = request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(key?.Value);
            var id = workspace?.Id ?? Guid.Empty;

            query = query
                .WhereEquals(x => x.WorkspaceId, id)
                .WhereEquals(x => x.Kind, Kind);

            return CoreHub.ObjectManager.GetObjects(query, context)
                .Select(x => new RestApiTileItem()
                {
                    Id = x.Id.ToString(),
                    Title = x.Summary,
                    Text = x.Description,
                    Image = x.Icon?.Uri?.ToString(),
                    PrimaryAction = GetPrimaryAction(x, request)?.ToJson()
                });
        }

        /// <summary>
        /// Applies the specified filter criteria to the given query object.
        /// </summary>
        /// <param name="filter">A string representing the filter expression to apply.</param>
        /// <param name="query">The query object to which the filter will be applied.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>A query representing the filtered set of items.</returns>
        protected override IQuery<Model.Entities.Object> Filter(string filter, IQuery<Model.Entities.Object> query, IRequest request)
        {
            if (string.IsNullOrWhiteSpace(filter) || filter == "null")
            {
                return query;
            }

            query = query.WhereContainsIgnoreCase
            (
                x => x.Summary, filter
            );

            return query;
        }

        /// <summary>
        /// Retrieves the primary action associated with the specified row item: opening its
        /// object detail page in the object-view frame.
        /// </summary>
        /// <param name="item">The object the action addresses.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The primary action.</returns>
        private static IAction GetPrimaryAction(Model.Entities.Object item, IRequest request)
        {
            return new ActionFrame("object-view-frame")
            {
                Uri = global::KleeneStar.Core.WebFragment.Object.ObjectKindCatalog
                        .ResolveDetailUri(item)
            };
        }
    }
}
