using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Represents an identity policy that grants view access to a workspace, including its content and metadata.
    /// </summary>
    [Name("workspace_view_policy")]
    [Permission<WorkspaceReadPermission>()]
    [Permission<WorkspaceReadContentPermission>()]
    public sealed class WorkspaceViewPolicy : IIdentityPolicy
    {
    }
}
