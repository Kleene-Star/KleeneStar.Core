using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that authorizes cloning of a workspace.
    /// </summary>
    [Name("workspace_clone")]
    [Policy<WorkspaceAdminPolicy>()]
    public sealed class WorkspaceClonePermission : IIdentityPermission
    {
    }

}
