using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling the archiving of an active form.
    /// </summary>
    [Name("form_archive")]
    [Policy<FormPublisherPolicy>()]
    [Policy<FormAdminPolicy>()]
    public sealed class FormArchivePermission : IIdentityPermission
    {
    }

}
