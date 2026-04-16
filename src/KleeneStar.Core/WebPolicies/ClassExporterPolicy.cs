using KleeneStar.Core.WebPermissions;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPolicies
{
    /// <summary>
    /// Policy enabling the export of class definitions for reuse or backup.
    /// </summary>
    [Name("class_exporter_policy")]
    [Permission<ClassExportPermission>()]
    public sealed class ClassExporterPolicy : IIdentityPolicy
    {
    }
}
