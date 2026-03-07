using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_
{
    /// <summary>
    /// Represents a REST API table for managing workspace entities, providing data retrieval 
    /// and option generation functionality for workspace records.
    /// </summary>
    [Title("kleenestar.core:object.tile.header")]
    [Cache]
    public sealed class Tile : RestApiTile<Model.Entities.Object>
    {
        private readonly IUri _editFormUri;
        private readonly IUri _cloneFormUri;
        private readonly IUri _deleteFormUri;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Tile()
        {
            //_editFormUri = CoreHub.GetUri<WWW.Workspaces._workspacekey_.Edit>();
            //_cloneFormUri = CoreHub.GetUri<WWW.Workspaces._workspacekey_.Clone>();
            //_deleteFormUri = CoreHub.GetUri<WWW.Workspaces._workspacekey_.Delete>();
        }

        /// <summary>
        /// Retrieves a collection of options.
        /// </summary>
        /// <param name="row">
        /// The row object for which options are being retrieved. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request object containing the criteria for retrieving options. Cannot be null.
        /// </param>
        public override IEnumerable<RestApiOption> GetOptions(Model.Entities.Object row, IRequest request)
        {
            //var editUri = _editFormUri?
            //    .BindParameters(new WorkspaceKeyParameter(row.Key));
            //var cloneUri = _cloneFormUri?
            //    .BindParameters(new WorkspaceKeyParameter(row.Key));
            //var deleteUri = _deleteFormUri?
            //    .BindParameters(new WorkspaceKeyParameter(row.Key));

            //yield return new RestApiOptionHeader(request)
            //{
            //    Text = "webexpress.webapp:header.setting.label"
            //};

            //yield return new RestApiOptionEdit(request)
            //{
            //    PrimaryAction = new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge)
            //};

            //yield return new RestApiOptionClone(request)
            //{
            //    PrimaryAction = new ActionModal("modal-form", cloneUri, TypeModalSize.ExtraLarge)
            //};

            //yield return new RestApiOptionCustom(request)
            //{
            //    Uri = CoreHub.GetUri<WWW.Workspaces._workspacekey_.Classes.Index>()?
            //        .BindParameters
            //        (
            //            new WorkspaceKeyParameter(row.Key)
            //        ),
            //    Text = I18N.Translate(request, "kleenestar.core:class.manage.label"),
            //    Icon = new ClassIcon()

            //};

            yield return new RestApiOptionSeparator(request);
            //yield return new RestApiOptionDelete(request)
            //{
            //    PrimaryAction = new ActionModal("modal-form", cloneUri, TypeModalSize.Small)
            //};
        }

        /// <summary>
        /// Retrieves the primary action associated with the specified 
        /// row item.
        /// </summary>
        /// <param name="item">
        /// The index item for which the inline‑edit REST API URI should be determined.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context for resolving
        /// the appropriate REST API URI.
        /// </param>
        /// <returns>
        /// An <see cref="IAction"/> representing the primary action for the specified 
        /// row item, or null if no action is available.
        /// </returns>
        public override IAction GetPrimaryAction(Model.Entities.Object item, IRequest request)
        {
            return new ActionFrame("object-view-frame")
            {
                Uri = CoreHub.GetUri<Object._objectkey_.Index>()?
                        .BindParameters(new ObjectKeyParameter(item.Key))
            };
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
        /// Retrieves a queryable collection of index items that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select index items. Cannot 
        /// be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{TIndexItem}"/> representing the filtered set of index items. The 
        /// result may be empty if no items match the query.
        /// </returns>
        protected override IEnumerable<Model.Entities.Object> Retrieve(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            return CoreHub.ObjectManager.GetObjects(query, context);
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
