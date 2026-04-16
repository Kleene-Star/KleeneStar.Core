using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing cloning of an existing workflow definition as a draft.
    /// </summary>
    [Name("workflow_clone")]
    [Policy<WorkflowEditPolicy>()]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowClonePermission : IIdentityPermission
    {
    }

}
