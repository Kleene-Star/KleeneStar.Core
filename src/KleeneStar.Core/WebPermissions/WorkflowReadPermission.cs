using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to workflow metadata, states, transitions,
    /// and version history.
    /// </summary>
    [Name("workflow_read")]
    [Policy<WorkflowViewPolicy>()]
    [Policy<WorkflowEditPolicy>()]
    [Policy<WorkflowPublisherPolicy>()]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowReadPermission : IIdentityPermission
    {
    }


}
