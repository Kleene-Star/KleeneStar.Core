using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that allows read-only access to workspace content.
    /// </summary>
    [Name("workspace_read_content")]
    [Policy<WorkspaceViewPolicy>()]
    [Policy<WorkspaceEditPolicy>()]
    [Policy<WorkspaceAdminPolicy>()]
    public sealed class WorkspaceReadContentPermission : IIdentityPermission
    {
    }

}
