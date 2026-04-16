using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling duplication of an existing class, including its structure and metadata.
    /// </summary>
    [Name("class_clone")]
    [Policy<ClassEditPolicy>()]
    [Policy<ClassAdminPolicy>()]
    public sealed class ClassClonePermission : IIdentityPermission
    {
    }

}
