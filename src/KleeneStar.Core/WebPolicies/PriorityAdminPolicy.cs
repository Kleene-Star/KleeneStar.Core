using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy providing full administrative control over priority management,
    /// including creation, modification, deletion, lifecycle operations,
    /// cloning, import/export, and permission administration.
    /// </summary>
    [Name("priority_admin_policy")]
    [Permission<PriorityCreatePermission>()]
    [Permission<PriorityReadPermission>()]
    [Permission<PriorityUpdatePermission>()]
    [Permission<PriorityDeletePermission>()]
    [Permission<PriorityArchivePermission>()]
    [Permission<PriorityRestorePermission>()]
    [Permission<PriorityClonePermission>()]
    [Permission<PriorityImportPermission>()]
    [Permission<PriorityExportPermission>()]
    public sealed class PriorityAdminPolicy : IIdentityPolicy
    {
    }
}
