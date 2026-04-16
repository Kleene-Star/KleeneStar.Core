using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that grants the ability to archive a workspace.
    /// </summary>
    [Name("workspace_archive")]
    [Policy<WorkspaceAdminPolicy>()]
    public sealed class WorkspaceArchivePermission : IIdentityPermission
    {
    }

}
