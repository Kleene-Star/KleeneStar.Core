using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Represents an identity policy that grants complete administrative control over a workspace.
    /// </summary>
    [Name("workspace_admin_policy")]
    [Permission<WorkspaceCreatePermission>()]
    [Permission<WorkspaceReadPermission>()]
    [Permission<WorkspaceUpdatePermission>()]
    [Permission<WorkspaceDeletePermission>()]
    [Permission<WorkspaceArchivePermission>()]
    [Permission<WorkspaceRestorePermission>()]
    [Permission<WorkspaceClonePermission>()]
    [Permission<WorkspaceManageProfilesPermission>()]
    [Permission<WorkspaceReadContentPermission>()]
    [Permission<WorkspaceWriteContentPermission>()]
    public sealed class WorkspaceAdminPolicy : IIdentityPolicy
    {
    }
}
