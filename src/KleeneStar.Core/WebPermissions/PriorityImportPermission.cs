using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling the import of external priority definitions.
    /// </summary>
    [Name("priority_import")]
    [Policy<PriorityImporterPolicy>()]
    [Policy<PriorityAdminPolicy>()]
    public sealed class PriorityImportPermission : IIdentityPermission
    {
    }

}
