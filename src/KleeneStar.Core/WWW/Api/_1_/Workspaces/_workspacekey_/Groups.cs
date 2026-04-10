using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Workspaces._workspacekey_
{
    /// <summary>
    /// Provides a selection of available groups for permission profile assignment within a workspace.
    /// </summary>
    [Title("Group selection")]
    [WorkspaceKeySegment]
    [Cache]
    public sealed class Groups : RestApiSelection<Workspace>
    {
        /// <summary>
        /// Applies the specified filter criteria to the given query object.
        /// </summary>
        /// <param name="filter">
        /// A string representing the filter expression to apply.
        /// </param>
        /// <param name="query">
        /// The query object to which the filter will be applied.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// A query representing the filtered set of items.
        /// </returns>
        protected override IQuery<Workspace> Filter(string filter, IQuery<Workspace> query, IRequest request)
        {
            if (string.IsNullOrWhiteSpace(filter) || filter == "null")
            {
                return query;
            }

            return query.WhereContainsIgnoreCase(x => x.Name, filter);
        }

        /// <summary>
        /// Retrieves the available groups for selection.
        /// </summary>
        /// <param name="query">
        /// The query parameters for filtering.
        /// </param>
        /// <param name="context">
        /// The query execution context.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// A queryable collection of selection items representing available groups.
        /// </returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Workspace> query, IQueryContext context, IRequest request)
        {
            var key = request.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(key);

            var list = new List<RestApiSelectionItem>();

            // retrieve groups from permission profiles assigned to the workspace
            if (workspace?.PermissionProfiles != null)
            {
                var existingGroupIds = new HashSet<Guid>();

                foreach (var profile in workspace.PermissionProfiles)
                {
                    if (profile.Group != null && existingGroupIds.Add(profile.Group.Id))
                    {
                        list.Add(new RestApiSelectionItem()
                        {
                            Id = profile.Group.Id,
                            Text = profile.Group.Name
                        });
                    }
                }
            }

            return list.AsQueryable();
        }
    }
}
