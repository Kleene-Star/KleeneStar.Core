using KleeneStar.Core.WebControl;
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
    /// Project-wide base for the object list endpoint of a kind's overview tab control:
    /// the workspace's objects of the <see cref="Kind"/> as a vertical frame list. A
    /// concrete subclass only fixes the kind it lists (issue, asset, …); each concrete
    /// endpoint registers at its own route, so the base must stay abstract (an endpoint
    /// that derived from another endpoint would shadow the base route).
    /// </summary>
    public abstract class RestApiObjectKindList : RestApiList<Model.Entities.Object>
    {
        /// <summary>
        /// Gets the persisted kind key the list is scoped to (e.g.
        /// <see cref="Model.Entities.ObjectKind.Issue"/>).
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
        /// Retrieves the list items: the workspace's objects of the endpoint's kind.
        /// </summary>
        /// <param name="query">The query (carries the applied search filter and paging).</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The list items, each opening its object detail page when selected.</returns>
        protected override IEnumerable<RestApiListItem> RetrieveItems(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            var key = request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(key?.Value);
            var id = workspace?.Id ?? Guid.Empty;

            query = query
                .WhereEquals(x => x.WorkspaceId, id)
                .WhereEquals(x => x.Kind, Kind);

            return CoreHub.ObjectManager.GetObjects(query, context)
                .Select(x => new RestApiListItem()
                {
                    Id = x.Id.ToString(),
                    Text = x.Summary,
                    Image = x.Icon?.Uri?.ToString(),
                    // the selection is handed to the master-detail composite rather than
                    // written into the frame, so it stays the single owner of the selection.
                    // the pane is fed the reduced view rather than the full reading view: the
                    // frame embeds a page's main content region, and that region of the reading
                    // view is written for a full-width column
                    PrimaryAction = new ActionMasterDetail(ListDetailControl.ControlId)
                    {
                        Uri = global::KleeneStar.Core.WebFragment.Object.ObjectKindCatalog
                            .ResolvePreviewUri(x)
                            .BindParameters(request),
                        Item = x.Id.ToString()
                    }.ToJson()
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
    }
}
