using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing permanent deletion of workflow drafts or archived versions.
    /// </summary>
    [Name("workflow_delete")]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class WorkflowDeletePermission : IIdentityPermission
    {
    }


}
