using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Represents an identity policy that governs permissions for editing workspace content.
    /// </summary>
    [Name("workspace_edit_policy")]
    [Permission<WorkspaceReadPermission>()]
    [Permission<WorkspaceReadContentPermission>()]
    [Permission<WorkspaceWriteContentPermission>()]
    public sealed class WorkspaceEditPolicy : IIdentityPolicy
    {
    }
}
