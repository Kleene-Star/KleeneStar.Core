using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy providing full administrative control over class definitions,
    /// including creation, modification, deletion, cloning, import/export,
    /// and permission management.
    /// </summary>
    [Name("class_admin_policy")]
    [Permission<ClassCreatePermission>()]
    [Permission<ClassReadPermission>()]
    [Permission<ClassUpdatePermission>()]
    [Permission<ClassDeletePermission>()]
    [Permission<ClassClonePermission>()]
    [Permission<ClassImportPermission>()]
    [Permission<ClassExportPermission>()]
    [Permission<ClassManagePermissionsPermission>()]
    public sealed class ClassAdminPolicy : IIdentityPolicy
    {
    }
}
