using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy authorizing workflow model maintenance without lifecycle control.
    /// Includes reading, updating, validating, cloning, and reading version history.
    /// </summary>
    [Name("workflow_edit_policy")]
    [Permission<WorkflowReadPermission>()]
    [Permission<WorkflowUpdatePermission>()]
    [Permission<WorkflowValidatePermission>()]
    [Permission<WorkflowClonePermission>()]
    [Permission<WorkflowVersionsReadPermission>()]
    public sealed class WorkflowEditPolicy : IIdentityPolicy
    {
    }
}
