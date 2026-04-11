using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Workspaces._workspacekey_
{
    /// <summary>
    /// Provides a selection of available policies for permission profile assignment within a workspace.
    /// </summary>
    [Title("Policy selection")]
    [Cache]
    public sealed class Policies : RestApiSelection<Workspace>
    {
        /// <summary>
        /// Creates a new instance of an object that implements the IQueryContext interface.
        /// </summary>
        /// <returns>
        /// An IQueryContext instance that can be used to execute queries.
        /// </returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

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
            return query;
        }

        /// <summary>
        /// Retrieves the available policies for selection.
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
        /// A queryable collection of selection items representing available policies.
        /// </returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Workspace> query, IQueryContext context, IRequest request)
        {
            var key = request.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(key);

            var list = new List<RestApiSelectionItem>();

            //// retrieve policies from permission profiles assigned to the workspace
            //if (workspace?.PermissionProfiles != null)
            //{
            //    var existingPolicyIds = new HashSet<Guid>();

            //    foreach (var profile in workspace.PermissionProfiles)
            //    {
            //        if (profile.Policy != null && existingPolicyIds.Add(profile.Policy.Id))
            //        {
            //            list.Add(new RestApiSelectionItem()
            //            {
            //                Id = profile.Policy.Id,
            //                Text = profile.Policy.Name
            //            });
            //        }
            //    }
            //}

            return list.AsQueryable();
        }
    }
}
