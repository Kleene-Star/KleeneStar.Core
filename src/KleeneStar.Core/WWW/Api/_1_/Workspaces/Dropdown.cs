using KleeneStar.Core.WebParameter.Workspace;
using KleeneStar.Model.Entity;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WWW.Api._1_.Workspaces
{
    [Title("Workspace")]
    [Cache]
    public sealed class Dropdown : RestApiDropdown<Workspace>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Dropdown()
        {
        }

        /// <summary>
        /// Gets the URI associated with the specified request and index item.
        /// </summary>
        /// <param name="request">
        /// The request for which to retrieve the URI. Cannot be null.
        /// </param>
        /// <param name="item">
        /// The index item that provides context for generating the URI. Cannot be null.
        /// </param>
        /// <returns>
        /// An object representing the URI for the given request and index item, or null if no URI is available.
        /// </returns>
        public override IUri GetUri(IRequest request, Workspace item)
        {
            return CoreHub.GetUri<WWW.Workspaces._key_.Index>()?
                .SetParameters
                (
                    new KeyParameter(item?.Key)
                );
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
        public override IEnumerable<Workspace> GetData(string filter, IRequest request)
        {
            var data = CoreHub.WorkspaceManager?.Workspaces;

            if (filter == null || filter == "null")
            {
                return data;
            }

            return data.Where
            (
                x => x.Name.Contains(filter, System.StringComparison.InvariantCultureIgnoreCase)
            );
        }
    }
}
