using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling the archiving of an active field.
    /// </summary>
    [Name("field_archive")]
    [Policy<FieldAdminPolicy>()]
    public sealed class FieldArchivePermission : IIdentityPermission
    {
    }

}
