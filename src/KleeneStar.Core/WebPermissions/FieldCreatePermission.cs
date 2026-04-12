using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the creation of new fields within a class.
    /// </summary>
    [Name("field_create")]
    [Policy<FieldCreatorPolicy>()]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldCreatePermission : IIdentityPermission
    {
    }

}
