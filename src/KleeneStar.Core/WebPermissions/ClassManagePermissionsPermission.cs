using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting access to manage class-level permission profiles and group-policy assignments.
    /// </summary>
    [Name("class_manage_permissions")]
    [Policy<ClassAdminPolicy>()]
    public sealed class ClassManagePermissionsPermission : IIdentityPermission
    {
    }

}
