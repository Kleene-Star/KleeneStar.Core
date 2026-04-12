using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission authorizing modifications to an existing class, including fields and configuration settings.
    /// </summary>
    [Name("class_update")]
    [Policy<ClassEditPolicy>()]
    [Policy<ClassAdminPolicy>()]
    public sealed class ClassUpdatePermission : IIdentityPermission
    {
    }

}
