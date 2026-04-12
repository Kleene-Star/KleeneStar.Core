using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that allows updating workspace settings or properties.
    /// </summary>
    [Name("workspace_update")]
    [Policy<WorkspaceAdminPolicy>()]
    public sealed class WorkspaceUpdatePermission : IIdentityPermission
    {
    }

}
