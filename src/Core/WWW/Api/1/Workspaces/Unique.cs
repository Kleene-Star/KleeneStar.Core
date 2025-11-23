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
    public sealed class Unique : RestApiUnique
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Unique()
        {
        }

        /// <summary>
        /// Determines whether the specified value is available based on the provided request context.
        /// </summary>
        /// <param name="value">
        /// The value to check for availability.
        /// </param>
        /// <param name="request">
        /// The request context containing additional information for the availability check. 
        /// </param>
        /// <returns>True if the specified value is available; otherwise, false.</returns>
        protected override bool CheckAvailable(string value, Request request)
        {
            var data = KleeneStar.WorkspaceManager?.Workspaces;
            var unique = data.Select(x => x.Name.ToLower()).Any(x => x.StartsWith(value));

            if (WebWorkspace.WorkspaceManager.ReservedWorkspaceKeys.Contains(value?.Trim().ToLower()))
            {
                return false;
            }

            return !unique;
        }
    }
}
