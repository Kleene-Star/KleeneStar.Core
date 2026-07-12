using System.Collections.Generic;
using WebExpress.WebCore.WebIdentity;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebPage;

namespace KleeneStar.Core.WebIdentity
{
    /// <summary>
    /// Represents an external identity provider that supplies identities and groups
    /// to the WebExpress identity system.
    /// </summary>
    public class IdentityProvider : WebExpress.WebApp.WebIdentity.IdentityProvider
    {
        /// <summary>
        /// Returns all identities provided by this source.
        /// </summary>
        public override IEnumerable<IIdentity> GetIdentities()
        {
            return [];
        }

        /// <summary>
        /// Returns all groups provided by this source.
        /// </summary>
        public override IEnumerable<IIdentityGroup> GetGroups()
        {
            return [];
        }

        /// <summary>
        /// Displays a login dialog using the specified request and identity information.
        /// </summary>
        /// <param name="request">
        /// The request containing parameters and context for the login operation. Cannot be null.
        /// </param>
        /// <param name="initiator">
        /// The endpoint that triggered the authentication process. Used to determine the origin and
        /// context of the authentication requirement.
        /// </param>
        /// <param name="identity">
        /// The identity information to be used for authentication. Cannot be null.
        /// </param>
        /// <returns>
        /// An object that represents the response to the login dialog, including authentication results and any
        /// relevant status information.
        /// </returns>
        public override IResponse CreateAuthenticationPrompt(IRequest request, IPageContext initiator, IIdentity identity)
        {
            var sessionUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Session>();

            return base.CreateAuthenticationPrompt(request, initiator, identity, sessionUri);
        }
    }
}
