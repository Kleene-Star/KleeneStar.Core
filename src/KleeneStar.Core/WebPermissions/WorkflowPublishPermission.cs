using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling publication of a reviewed workflow draft as the active version.
    /// </summary>
    [Name("workflow_publish")]
    [Policy<WorkflowPublisherPolicy>()]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowPublishPermission : IIdentityPermission
    {
    }

}
