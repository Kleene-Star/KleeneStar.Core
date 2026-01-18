using KleeneStar.Core.WebParameter.Workspace;
using KleeneStar.Model.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Core.WWW.Api._1.Workspaces
{
    /// <summary>
    /// Represents a REST API table for managing workspace entities, providing data retrieval 
    /// and option generation functionality for workspace records.
    /// </summary>
    [Title("Workspace")]
    [Cache]
    public sealed class Table : RestApiTable<IWorkspace>
    {
        private readonly IUri _editFormUri;
        private readonly IUri _deleteFormUri;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Table()
        {
            _editFormUri = CoreHub.GetUri<WWW.Workspace.Id.Edit>();
            _deleteFormUri = CoreHub.GetUri<WWW.Workspace.Id.Delete>();
        }

        /// <summary>
        /// Retrieves a collection of options.
        /// </summary>
        /// <param name="request">
        /// The request object containing the criteria for retrieving options. Cannot be null.
        /// </param>
        /// <param name="row">
        /// The row object for which options are being retrieved. Cannot be null.
        /// </param>
        public override IEnumerable<RestApiOption> GetOptions(IRequest request, IWorkspace row)
        {
            yield return new RestApiOptionHeader(request)
            {
                Label = "webexpress.webapp:header.setting.label"
            };

            yield return new RestApiOptionEdit(request)
            {
                Uri = _editFormUri?.SetParameters
                (
                    new KeyParameter(row.Key)
                )?
                    .ToString(),
                Modal = new ModalTarget("modal-form", TypeModalSize.ExtraLarge)
            };

            yield return new RestApiOptionSeperator(request);
            yield return new RestApiOptionDelete(request)
            {
                Uri = _deleteFormUri?.SetParameters
                (
                    new KeyParameter(row.Key)
                )?
                    .ToString(),
                Modal = new ModalTarget("modal-form", TypeModalSize.Small)
            };
        }

        /// <summary>
        /// Retrieves a URI that represents the specified request within the given workspace context.
        /// </summary>
        /// <param name="request">
        /// The request for which to obtain the corresponding URI. Cannot be null.
        /// </param>
        /// <param name="row">
        /// The workspace context in which the request is evaluated. Cannot be null.
        /// </param>
        /// <returns>
        /// An object implementing <see cref="IUri"/> that represents the URI for the specified request and workspace.
        /// </returns>
        public override IUri GetUri(IRequest request, IWorkspace row)
        {
            return CoreHub.GetUri<WWW.Index>()?
                .Concat(row.Key);
        }

        /// <summary>
        /// Returns the REST API endpoint URI associated with the specified request and workspace.
        /// </summary>
        /// <param name="request">
        /// The request for which to retrieve the REST API endpoint.
        /// </param>
        /// <param name="row">
        /// The workspace context used to determine the appropriate REST API endpoint.
        /// </param>
        /// <returns>
        /// An object representing the URI of the REST API endpoint for the given request and workspace.
        /// </returns>
        public override IUri GetRestApiForInlineEdit(IRequest request, IWorkspace row)
        {
            return CoreHub.GetUri<WWW.Api._1.Workspaces.Index>()?
                .Add(new UriQuery("id", row.Id.ToString()));
        }

        /// <summary>
        /// Retrieves a collection of objects based on the specified WQL statement and request.
        /// </summary>
        /// <param name="filter">
        /// The filter used to query the data. This parameter defines the filtering and 
        /// selection criteria.
        /// </param>
        /// <param name="request">
        /// The request context containing additional information for the operation.
        /// </param>
        /// <returns>
        /// An enumerable containing the objects that match the query criteria.
        /// </returns>
        public override IEnumerable<IWorkspace> GetData(string filter, IRequest request)
        {
            var data = CoreHub.WorkspaceManager?.Workspaces;

            if (request.GetParameter<CategoryParameter>() is Parameter category)
            {
                data = data.Where
                (
                    x => x.Categories
                        .Select(x => x.ToLower())
                        .Contains(category.Value?.ToLower())
                );
            }

            if (filter == null || filter == "null")
            {
                return data;
            }

            return data.Where
            (
                x => x.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)
            );
        }
    }
}
