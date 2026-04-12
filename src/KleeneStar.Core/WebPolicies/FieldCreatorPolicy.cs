using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Global policy allowing the creation of new fields within a class.
    /// </summary>
    [Name("field_creator_policy")]
    [Permission<FieldCreatePermission>()]
    public sealed class FieldCreatorPolicy : IIdentityPolicy
    {
    }
}
