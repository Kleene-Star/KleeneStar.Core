using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that allows deleting a workspace within the system.
    /// </summary>
    [Name("workspace_delete")]
    [Policy<WorkspaceAdminPolicy>()]
    public sealed class WorkspaceDeletePermission : IIdentityPermission
    {
    }

}
