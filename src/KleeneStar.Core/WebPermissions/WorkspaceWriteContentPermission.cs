using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that grants the ability to modify content within a workspace.
    /// </summary>
    [Name("workspace_write_content")]
    [Policy<WorkspaceEditPolicy>()]
    [Policy<WorkspaceAdminPolicy>()]
    public sealed class WorkspaceWriteContentPermission : IIdentityPermission
    {
    }

}
