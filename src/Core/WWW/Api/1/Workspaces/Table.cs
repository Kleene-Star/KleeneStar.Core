using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebWorkspace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebRestApi;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WWW.Api._1.Workspaces
{
    [Title("Workspace")]
    [Method(CrudMethod.GET)]
    [Method(CrudMethod.DELETE)]
    [Method(CrudMethod.PUT)]
    [Cache]
    public sealed class Table : RestApiCrudTable<IWorkspace>
    {
        private readonly IUri _formUri;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Table()
        {
            var uri = KleeneStar.GetUri<WWW.WorkspaceManager.Entity.Edit>();
            _formUri = new UriEndpoint(uri?.SetFragment("kleenestar-workspace-form-edit"));

            Data = KleeneStar.WorkspaceManager?.Workspaces;
        }

        /// <summary>
        /// Retrieves a collection of options.
        /// </summary>
        /// <param name="request">The request object containing the criteria for retrieving options. Cannot be null.</param>
        /// <param name="row">The row object for which options are being retrieved. Cannot be null.</param>
        public override IEnumerable<RestApiCrudOption> GetOptions(Request request, IWorkspace row)
        {
            var uri = new UriEndpoint(_formUri);
            uri = uri.SetParameters(new KeyParameter(row.Key)) as UriEndpoint;

            yield return new RestApiCrudOptionHeader(request)
            {
                Label = "webexpress.webapp:header.setting.label"
            };

            yield return new RestApiCrudOptionEdit(request)
            {
                Uri = uri.ToString()
            };

            yield return new RestApiCrudOptionSeperator(request);
            yield return new RestApiCrudOptionDelete(request);
        }

        /// <summary>
        /// Retrieves a collection of objects based on the specified WQL statement and request.
        /// </summary>
        /// <param name="filter">
        /// The filter used to query the data. This parameter defines the filtering and selection criteria.
        /// </param>
        /// <param name="request">
        /// The request context containing additional information for the operation.
        /// </param>
        /// <returns>
        /// An enumerable containing the objects that match the query criteria.
        /// </returns>
        public override IEnumerable<IWorkspace> GetData(string filter, Request request)
        {
            var data = Data;

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
                return Data;
            }

            return data
                .Where
                (
                    x => x.Name.Contains(filter, System.StringComparison.InvariantCultureIgnoreCase)
                );
        }

        /// <summary>
        /// Performs validation before updating data.
        /// </summary>
        /// <param name="item"> The item containing the updated data.</param>
        /// <param name="request">The HTTP request containing input data and parameters.</param>
        /// <returns>
        /// A <see cref="RestApiValidationResult"/> containing any validation errors 
        /// encountered during the update process. If the operation completes successfully, 
        /// the result will contain no errors.
        /// </returns>
        public override RestApiValidationResult ValidateUpdateData(IWorkspace item, Request request)
        {
            return new RestApiValidator(request)
                .Require(nameof(IWorkspace.Name))
                .MinLength(nameof(IWorkspace.Name), 3)
                .Result;
        }

        /// <summary>
        /// Updates the data record identified by the specified ID.
        /// </summary>
        /// <param name="item"> The item containing the updated data.</param>
        /// <param name="request">The HTTP request containing the update parameters.</param>
        public override void UpdateData(IWorkspace item, Request request)
        {
            var i = item as Workspace;
            i.Name = request.GetParameter(nameof(IWorkspace.Name))?.Value;
        }

        /// <summary>
        /// Deletes data.
        /// </summary>
        /// <param name="id">The id of the data to delete.</param>
        /// <param name="request">The request.</param>
        public override void DeleteData(string id, Request request)
        {
            var guid = default(Guid);
            Guid.TryParse(id, out guid);

            if (guid != Guid.Empty)
            {
                KleeneStar.WorkspaceManager.RemoveWorkspace(guid);
            }
        }
    }
}
