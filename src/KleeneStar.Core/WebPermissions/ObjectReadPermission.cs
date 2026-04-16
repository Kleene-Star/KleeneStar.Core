using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to an object's metadata and visible fields.
    /// Effective only if workspace, class, and field read permissions are also satisfied.
    /// </summary>
    [Name("object_read")]
    [Policy<ObjectViewPolicy>()]
    [Policy<ObjectEditPolicy>()]
    [Policy<ObjectAdminPolicy>()]
    public sealed class ObjectReadPermission : IIdentityPermission
    {
    }

}
