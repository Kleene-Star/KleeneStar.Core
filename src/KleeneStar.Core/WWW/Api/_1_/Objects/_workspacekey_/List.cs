using KleeneStar.Core.WebControl;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_
{
    /// <summary>
    /// Provides a REST API list of objects within a specific workspace and enables
    /// filtering, retrieving, and managing class objects through API requests.
    /// </summary>
    [Title("kleenestar.core:object.list.header")]
    [Cache]
    public sealed class List : RestApiList<Model.Entities.Object>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public List()
        {
        }

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
        /// Retrieves a collection of tile items representing classes that match the 
        /// specified query and workspace context.
        /// </summary>
        /// <param name="query">
        /// The query used to filter classes. The query is further constrained to the 
        /// workspace identified by the request parameters.
        /// </param>
        /// <param name="context">
        /// The context for the query execution, providing additional information or 
        /// services required to process the query.
        /// </param>
        /// <param name="request">
        /// The current API request, used to extract workspace identification 
        /// parameters.
        /// </param>
        /// <returns>
        /// An enumerable collection of list items representing the classes that 
        /// satisfy the query and belong to the specified workspace. The collection 
        /// is empty if no matching classes are found.
        /// </returns>
        protected override IEnumerable<RestApiListItem> RetrieveItems(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            var key = request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(key?.Value);
            var id = workspace?.Id ?? Guid.Empty;

            // the tab views live on the issue overview, so they present the issue kind only
            query = query
                .WhereEquals(x => x.WorkspaceId, id)
                .WhereEquals(x => x.Kind, Model.Entities.ObjectKind.Issue);

            return CoreHub.ObjectManager.GetObjects(query, context)
                .Select(x => new RestApiListItem()
                {
                    Id = x.Id.ToString(),
                    Text = x.Summary,
                    Image = x.Icon?.Uri?.ToString(),
                    PrimaryAction = new ActionFrame(ListDetailControl.FrameId)
                    {
                        Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Object._objectkey_.Index>()
                            .BindParameters(new ObjectKeyParameter(x.Key))
                            .BindParameters(request)
                    }.ToJson()
                });
        }

        /// <summary>
        /// Applies the specified filter criteria to the given query object.
        /// </summary>
        /// <param name="filter">
        /// A string representing the filter expression to apply. The format and supported 
        /// operators depend on the implementation.
        /// </param>
        /// <param name="query">
        /// The query object to which the filter will be applied.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context for resolving
        /// the appropriate REST API URI.
        /// </param>
        /// <returns>
        /// A query representing the filtered set of items that match the criteria defined by 
        /// the filter statement.
        /// </returns>
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
