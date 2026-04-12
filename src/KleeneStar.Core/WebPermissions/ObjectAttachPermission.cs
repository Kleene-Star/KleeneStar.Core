using KleeneStar.Core.WebPolicies;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WebPermissions
{
    /// <summary>
    /// Permission enabling uploading, modifying, and deleting attachments associated with an object.
    /// </summary>
    [Name("object_attach")]
    [Policy<ObjectEditPolicy>()]
    [Policy<ObjectAdminPolicy>()]
    public sealed class ObjectAttachPermission : IIdentityPermission
    {
    }

}
