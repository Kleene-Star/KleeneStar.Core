using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to the usage proof of a status.
    /// </summary>
    [Name("status_usage_read")]
    [Policy<StatusAdminPolicy>()]
    public sealed class StatusUsageReadPermission : IIdentityPermission
    {
    }

}
