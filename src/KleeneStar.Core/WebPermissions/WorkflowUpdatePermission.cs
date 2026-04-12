using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing modifications to workflow drafts, including states,
    /// transitions, guards, validators, and post functions.
    /// </summary>
    [Name("workflow_update")]
    [Policy<WorkflowEditPolicy>()]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowUpdatePermission : IIdentityPermission
    {
    }

}
