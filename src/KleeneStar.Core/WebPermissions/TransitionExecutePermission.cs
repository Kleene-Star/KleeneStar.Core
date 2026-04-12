using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing execution of transitions on objects.
    /// May be assigned workflow-wide or transition-specific.
    /// </summary>
    [Name("transition_execute")]
    [Policy<WorkflowAdminPolicy>()]
    public sealed class TransitionExecutePermission : IIdentityPermission
    {
    }

}
