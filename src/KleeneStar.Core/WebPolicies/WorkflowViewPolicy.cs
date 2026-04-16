using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting read-only access to workflow metadata and version history.
    /// </summary>
    [Name("workflow_view_policy")]
    [Permission<WorkflowReadPermission>()]
    [Permission<WorkflowVersionsReadPermission>()]
    public sealed class WorkflowViewPolicy : IIdentityPolicy
    {
    }
}
