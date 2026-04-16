using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Represents an identity policy that grants permission to create workspaces.
    /// </summary>
    [Name("workspace_creator_policy")]
    [Permission<WorkspaceCreatePermission>()]
    public sealed class WorkspaceCreatorPolicy : IIdentityPolicy
    {
    }
}
