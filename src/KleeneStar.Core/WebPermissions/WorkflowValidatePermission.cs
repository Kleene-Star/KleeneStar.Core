using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing execution of workflow validation checks and access
    /// to validation reports.
    /// </summary>
    [Name("workflow_validate")]
    [Policy<WorkflowEditPolicy>()]
    [Policy<WorkflowPublisherPolicy>()]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowValidatePermission : IIdentityPermission
    {
    }

}
