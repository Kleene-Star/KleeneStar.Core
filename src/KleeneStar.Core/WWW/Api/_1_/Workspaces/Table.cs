using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Api._1_.Workspaces
{
    /// <summary>
    /// Represents a REST API table for managing workspace entities, providing data retrieval 
    /// and option generation functionality for workspace records.
    /// </summary>
    [Title("kleenestar.core:workspace.table.header")]
    [Cache]
    public sealed class Table : RestApiTable<Workspace>
    {
        private readonly IUri _editFormUri;
        private readonly IUri _cloneFormUri;
        private readonly IUri _deleteFormUri;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Table()
        {
            _editFormUri = CoreHub.GetUri<WWW.Workspaces._key_.Edit>();
            _cloneFormUri = CoreHub.GetUri<WWW.Workspaces._key_.Clone>();
            _deleteFormUri = CoreHub.GetUri<WWW.Workspaces._key_.Delete>();
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
        public override IEnumerable<RestApiOption> GetOptions(Workspace row, IRequest request)
        {
            var editUri = _editFormUri?
                .BindParameters(new KeyParameter(row.Key));
            var cloneUri = _cloneFormUri?
                .BindParameters(new KeyParameter(row.Key));
            var deleteUri = _deleteFormUri?
                .BindParameters(new KeyParameter(row.Key));

            yield return new RestApiOptionHeader(request)
            {
                Text = "webexpress.webapp:header.setting.label"
            };

            yield return new RestApiOptionEdit(request)
            {
                PrimaryAction = new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge)
            };

            yield return new RestApiOptionClone(request)
            {
                PrimaryAction = new ActionModal("modal-form", cloneUri, TypeModalSize.ExtraLarge)
            };

            yield return new RestApiOptionCustom(request)
            {
                Uri = CoreHub.GetUri<WWW.Workspaces._key_.Classes.Index>()?
                    .BindParameters
                    (
                        new KeyParameter(row.Key)
                    ),
                Text = I18N.Translate(request, "kleenestar.core:class.manage.label"),
                Icon = new IconBoxesStacked().Class
            };

            yield return new RestApiOptionSeparator(request);
            yield return new RestApiOptionDelete(request)
            {
                PrimaryAction = new ActionModal("modal-form", deleteUri, TypeModalSize.Small)
            };
        }

        /// <summary>
        /// Retrieves a URI that represents the specified request within the given workspace context.
        /// </summary>
        /// <param name="row">
        /// The workspace context in which the request is evaluated. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request for which to obtain the corresponding URI. Cannot be null.
        /// </param>
        /// <returns>
        /// An object implementing <see cref="IUri"/> that represents the URI for the specified request and workspace.
        /// </returns>
        public override IUri GetUri(Workspace row, IRequest request)
        {
            return CoreHub.GetUri<WWW.Workspaces.Index>()?
                .Concat(row.Key);
        }

        /// <summary>
        /// Returns the REST API endpoint URI associated with the specified request and workspace.
        /// </summary>
        /// <param name="row">
        /// The workspace context used to determine the appropriate REST API endpoint.
        /// </param>
        /// <param name="request">
        /// The request for which to retrieve the REST API endpoint.
        /// </param>
        /// <returns>
        /// An object representing the URI of the REST API endpoint for the given request and workspace.
        /// </returns>
        public override IUri GetRestApiForInlineEdit(Workspace row, IRequest request)
        {
            return CoreHub.GetUri<_key_.Index>()?
                .Add(new UriQuery("id", row.Id.ToString()));
        }

        /// <summary>
        /// Retrieves the primary action associated with the specified 
        /// workspace and request.
        /// </summary>
        /// <param name="row">
        /// The workspace instance that provides the context for determining 
        /// the primary action.
        /// </param>
        /// <param name="request">
        /// The request object that may influence the selection of the 
        /// primary action.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IAction"/> representing the primary 
        /// action for the given workspace and request.
        /// </returns>
        public override IAction GetPrimaryAction(Workspace row, IRequest request)
        {
            return null;
        }

        /// <summary>
        /// Retrieves the secondary action associated with the specified 
        /// workspace and request.
        /// </summary>
        /// <param name="row">
        /// The workspace instance that provides the context for determining 
        /// the primary action.
        /// </param>
        /// <param name="request">
        /// The request object that may influence the selection of the 
        /// primary action.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IAction"/> representing the primary 
        /// action for the given workspace and request.
        /// </returns>
        public override IAction GetSecondaryAction(Workspace row, IRequest request)
        {
            var editUri = _editFormUri?
                .BindParameters(new KeyParameter(row.Key));

            return new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge);
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
        protected override IEnumerable<Workspace> Retrieve(IQuery<Workspace> query, IQueryContext context, IRequest request)
        {
            return CoreHub.WorkspaceManager.GetWorkspaces(query, context);
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
        protected override IQuery<Workspace> Filter(string filter, IQuery<Workspace> query, IRequest request)
        {
            if (string.IsNullOrWhiteSpace(filter) || filter == "null")
            {
                return query;
            }

            query = query.WhereContainsIgnoreCase
            (
                x => x.Name, filter
            );

            if (request.GetParameter<CategoryParameter>() is Parameter category)
            {
                query = query.WhereContainsIgnoreCase
                (
                    x => x.Categories.Select(x => x.Name),
                    category.Value
                );
            }

            return query;
        }
    }
}
