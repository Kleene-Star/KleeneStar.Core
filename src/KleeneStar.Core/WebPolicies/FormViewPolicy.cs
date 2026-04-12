using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy granting read-only access to form metadata,
    /// structure, and assignments.
    /// </summary>
    [Name("form_view_policy")]
    [Permission<FormReadPermission>()]
    public sealed class FormViewPolicy : IIdentityPolicy
    {
    }
}
