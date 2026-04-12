using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing deletion of a status that is not referenced
    /// by any workflow or object.
    /// </summary>
    [Name("status_delete")]
    [Policy<StatusAdminPolicy>()]
    public sealed class StatusDeletePermission : IIdentityPermission
    {
    }

}
