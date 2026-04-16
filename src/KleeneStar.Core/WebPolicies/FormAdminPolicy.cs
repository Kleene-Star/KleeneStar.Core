using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy providing full administrative control over form management,
    /// including creation, modification, deletion, lifecycle operations,
    /// transition assignments, import/export, and permission administration.
    /// </summary>
    [Name("form_admin_policy")]
    [Permission<FormCreatePermission>()]
    [Permission<FormReadPermission>()]
    [Permission<FormUpdatePermission>()]
    [Permission<FormDeletePermission>()]
    [Permission<FormArchivePermission>()]
    [Permission<FormRestorePermission>()]
    [Permission<FormClonePermission>()]
    [Permission<FormAssignTransitionPermission>()]
    [Permission<FormImportPermission>()]
    [Permission<FormExportPermission>()]
    public sealed class FormAdminPolicy : IIdentityPolicy
    {
    }
}
