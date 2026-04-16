using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to workflow version lists, diffs, and history.
    /// </summary>
    [Name("workflow_versions_read")]
    [Policy<WorkflowViewPolicy>()]
    [Policy<WorkflowEditPolicy>()]
    [Policy<WorkflowPublisherPolicy>()]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowVersionsReadPermission : IIdentityPermission
    {
    }

}
