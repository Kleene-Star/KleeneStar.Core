using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling cloning of an existing status.
    /// </summary>
    [Name("status_clone")]
    [Policy<StatusAdminPolicy>()]
    public sealed class StatusClonePermission : IIdentityPermission
    {
    }

}
