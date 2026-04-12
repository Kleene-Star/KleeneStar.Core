using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting read access to the status catalog of a class.
    /// </summary>
    [Name("status_read")]
    [Policy<StatusAdminPolicy>()]
    public sealed class StatusReadPermission : IIdentityPermission
    {
    }
}
