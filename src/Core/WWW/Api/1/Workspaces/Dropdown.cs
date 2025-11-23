using KleeneStar.Core.WebWorkspace;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace KleeneStar.Core.WWW.Api._1.Workspaces
{
    [Title("Workspace")]
    [Method(CrudMethod.GET)]
    [Cache]
    public sealed class Dropdown : RestApiCrudDropdown<IWorkspace>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Dropdown()
        {
            Data = KleeneStar.WorkspaceManager?.Workspaces;
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
            if (filter == null || filter == "null")
            {
                return Data;
            }

            return Data
                .Where
                (
                    x => x.Name.Contains(filter, System.StringComparison.InvariantCultureIgnoreCase)
                );
        }
    }
}
