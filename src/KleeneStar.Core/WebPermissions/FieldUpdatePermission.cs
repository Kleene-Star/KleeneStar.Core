using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing modifications to an existing field definition,
    /// including its configuration, metadata, and structural properties.
    /// </summary>
    [Name("field_update")]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldUpdatePermission : IIdentityPermission
    {
    }

}
