using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy providing full administrative control over a field,
    /// including creation, modification, deletion, cloning, archiving,
    /// restoration, value management, and permission administration.
    /// </summary>
    [Name("field_admin_policy")]
    [Permission<FieldCreatePermission>()]
    [Permission<FieldReadPermission>()]
    [Permission<FieldUpdatePermission>()]
    [Permission<FieldDeletePermission>()]
    [Permission<FieldArchivePermission>()]
    [Permission<FieldRestorePermission>()]
    [Permission<FieldClonePermission>()]
    [Permission<FieldManagePermissionsPermission>()]
    [Permission<FieldReadValuesPermission>()]
    [Permission<FieldWriteValuesPermission>()]
    public sealed class FieldAdminPolicy : IIdentityPolicy
    {
    }
}
