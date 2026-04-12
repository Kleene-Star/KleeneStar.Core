using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing updates to an existing object.
    /// Actual write access depends on field-level write permissions.
    /// </summary>
    [Name("object_update")]
    [Policy<ObjectEditPolicy>()]
    [Policy<ObjectAdminPolicy>()]
    public sealed class ObjectUpdatePermission : IIdentityPermission
    {
    }

}
