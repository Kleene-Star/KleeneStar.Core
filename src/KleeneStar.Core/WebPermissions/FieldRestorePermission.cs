using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the restoration of a previously archived field.
    /// </summary>
    [Name("field_restore")]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldRestorePermission : IIdentityPermission
    {
    }

}
