using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing the cloning of an existing field,
    /// including its configuration and metadata.
    /// </summary>
    [Name("field_clone")]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldClonePermission : IIdentityPermission
    {
    }

}
