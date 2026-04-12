using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the permanent deletion of a field from a class.
    /// </summary>
    [Name("field_delete")]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldDeletePermission : IIdentityPermission
    {
    }

}
