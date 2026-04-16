using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling the import of external form definitions.
    /// </summary>
    [Name("form_import")]
    [Policy<FormImporterPolicy>()]
    [Policy<FormAdminPolicy>()]
    public sealed class FormImportPermission : IIdentityPermission
    {
    }

}
