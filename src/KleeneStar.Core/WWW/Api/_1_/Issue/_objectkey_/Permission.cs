using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebPermission;
using KleeneStar.Core.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Issue._objectkey_
{
    /// <summary>
    /// Serves the permission dialog of a single object: which group holds which policy on it.
    /// </summary>
    [IncludeSubPaths]
    [Cache]
    public sealed class Permission : RestApiPermissionScoped
    {
        /// <summary>
        /// Gets the kind of resource this endpoint administers.
        /// </summary>
        protected override string Scope => PermissionScope.Object;

        /// <summary>
        /// Returns the object the request addresses.
        /// </summary>
        /// <remarks>
        /// The grants are keyed by the object's id rather than by the key in the route, so moving
        /// an object between workspaces — which changes its key — does not orphan them.
        /// </remarks>
        /// <param name="request">The request whose route names the object.</param>
        /// <returns>The object id, or null when the route addresses none.</returns>
        protected override string ResolveScopeId(IRequest request)
        {
            var key = request?.GetParameter<ObjectKeyParameter>()?.Value;

            return CoreHub.ObjectManager.GetObjectByKey(key)?.Id.ToString();
        }
    }
}
