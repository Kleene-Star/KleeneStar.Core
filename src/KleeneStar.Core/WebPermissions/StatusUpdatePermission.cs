using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing updates to an existing status,
    /// including name, category, and description.
    /// </summary>
    [Name("status_update")]
    [Policy<StatusAdminPolicy>()]
    public sealed class StatusUpdatePermission : IIdentityPermission
    {
    }

}
