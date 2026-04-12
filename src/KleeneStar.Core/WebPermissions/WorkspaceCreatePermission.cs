using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that allows a user or identity to create new workspaces.
    /// </summary>
    [Name("workspace_create")]
    [Policy<WorkspaceCreatorPolicy>()]
    public sealed class WorkspaceCreatePermission : IIdentityPermission
    {
    }

}
