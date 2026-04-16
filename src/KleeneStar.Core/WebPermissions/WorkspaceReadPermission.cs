using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that grants read-only access to a workspace.
    /// </summary>
    [Name("workspace_read")]
    [Policy<WorkspaceViewPolicy>()]
    [Policy<WorkspaceEditPolicy>()]
    [Policy<WorkspaceAdminPolicy>()]
    public sealed class WorkspaceReadPermission : IIdentityPermission
    {
    }

}
