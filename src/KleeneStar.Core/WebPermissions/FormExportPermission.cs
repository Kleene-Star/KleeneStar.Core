using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the export of form definitions,
    /// including structure and metadata.
    /// </summary>
    [Name("form_export")]
    [Policy<FormPublisherPolicy>()]
    [Policy<FormExporterPolicy>()]
    [Policy<FormAdminPolicy>()]
    public sealed class FormExportPermission : IIdentityPermission
    {
    }

}
