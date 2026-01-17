using KleeneStar.Core.Model.Workspace;
using KleeneStar.Core.WebParameter.Workspace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;

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
            _editFormUri = KleeneStar.GetUri<WWW.Workspace.Entity.Edit>();
            _deleteFormUri = KleeneStar.GetUri<WWW.Workspace.Entity.Delete>();
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
                Modal = "#modal-form"
            };

            yield return new RestApiOptionSeperator(request);
            yield return new RestApiOptionDelete(request)
            {
                Uri = _deleteFormUri?.SetParameters
                (
                    new KeyParameter(row.Key)
                )?
                    .ToString(),
                Modal = "#modal-form"
            };
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
            var data = KleeneStar.WorkspaceManager?.Workspaces;

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
