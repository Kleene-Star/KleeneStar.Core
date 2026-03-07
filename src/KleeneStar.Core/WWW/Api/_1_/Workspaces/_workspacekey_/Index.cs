using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebParameter;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebRestApi;

namespace KleeneStar.Core.WWW.Api._1_.Workspaces._workspacekey_
{
    /// <summary>
    /// Provides CRUD operations for workspace items via a REST API.
    /// </summary>
    [Cache]
    [WorkspaceKeySegment<WorkspaceKeyParameter>()]
    public sealed class Index : IRestApi
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }
    }
}
