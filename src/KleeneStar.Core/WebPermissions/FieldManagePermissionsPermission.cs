using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting access to manage field-level permission profiles,
    /// including assigning policies to groups.
    /// </summary>
    [Name("field_manage_permissions")]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldManagePermissionsPermission : IIdentityPermission
    {
    }

}
