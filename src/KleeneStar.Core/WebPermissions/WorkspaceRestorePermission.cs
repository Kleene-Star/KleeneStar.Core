using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that allows restoring a workspace.
    /// </summary>
    [Name("workspace_restore")]
    [Policy<WorkspaceAdminPolicy>()]
    public sealed class WorkspaceRestorePermission : IIdentityPermission
    {
    }

}
