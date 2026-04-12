using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Represents a permission that allows the management of profiles (assignment of policies to groups) for a workspace.
    /// </summary>
    [Name("workspace_manage_profiles")]
    [Policy<WorkspaceAdminPolicy>()]
    public sealed class WorkspaceManageProfilesPermission : IIdentityPermission
    {
    }

}
