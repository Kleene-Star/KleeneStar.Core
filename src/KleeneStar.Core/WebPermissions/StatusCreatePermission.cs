using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing creation of new statuses.
    /// </summary>
    [Name("status_create")]
    [Policy<StatusAdminPolicy>()]
    public sealed class StatusCreatePermission : IIdentityPermission
    {
    }

}
