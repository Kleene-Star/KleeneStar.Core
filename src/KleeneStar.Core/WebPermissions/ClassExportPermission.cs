using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission allowing the export of class definitions for reuse, documentation, or backup.
    /// </summary>
    [Name("class_export")]
    [Policy<ClassExporterPolicy>()]
    [Policy<ClassAdminPolicy>()]
    public sealed class ClassExportPermission : IIdentityPermission
    {
    }

}
