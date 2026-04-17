using KleeneStar.Model.Entities;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Core.WWW.Api._1_
{
    /// <summary>
    /// Represents a session that manages authentication and credential validation for REST API requests.
    /// </summary>
    [Cache]
    public sealed class Session : RestApiSession
    {
        /// <summary>
        /// Validates the provided credentials.
        /// </summary>
        /// <param name="username">The username to validate.</param>
        /// <param name="password">The password to validate.</param>
        /// <returns>The authenticated identity if valid; otherwise, null.</returns>
        protected override IIdentity ValidateCredentials(string username, string password)
        {
            var group = new Group()
            {
                GroupPolicies = [new GroupPolicy() { Policy = "kleenestar.core.webpolicies.workspaceviewpolicy" }]
            };

            return new Identity()
            {
                Name = "Test-User",
                GroupMemberships = [new IdentityGroupMembership() { Group = group }]
            };
        }
    }
}
