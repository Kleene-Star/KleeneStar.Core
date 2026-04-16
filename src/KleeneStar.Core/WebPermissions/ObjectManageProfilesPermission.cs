using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission granting access to manage object-level profiles,
    /// including assignment of object policies to global groups.
    /// </summary>
    [Name("object_manage_profiles")]
    [Policy<ObjectAdminPolicy>()]
    public sealed class ObjectManageProfilesPermission : IIdentityPermission
    {
    }

}
