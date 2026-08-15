using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing the devices and browsers that are currently signed in
    /// with an identity.
    /// </summary>
    public interface IIdentitySessionManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when a session is ended.
        /// </summary>
        event EventHandler<IdentitySession> IdentitySessionRemoved;

        /// <summary>
        /// Returns the sessions of the identity the request is served for, the current device
        /// first.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>An enumerable collection of sessions (possibly empty).</returns>
        IEnumerable<IdentitySession> GetSessions(IRequest request);

        /// <summary>
        /// Returns the sessions of the given identity, the current device first.
        /// </summary>
        /// <param name="ownerId">The signed-in identity.</param>
        /// <returns>An enumerable collection of sessions (possibly empty).</returns>
        IEnumerable<IdentitySession> GetSessions(Guid ownerId);

        /// <summary>
        /// Returns a session by its id.
        /// </summary>
        /// <param name="sessionId">The id of the session.</param>
        /// <returns>The session, or <see langword="null"/> when no such session exists.</returns>
        IdentitySession GetSession(Guid sessionId);

        /// <summary>
        /// Ends the session with the given id, signing the account out on that device.
        /// </summary>
        /// <param name="sessionId">The id of the session to end.</param>
        /// <returns>The current instance for method chaining.</returns>
        IIdentitySessionManager Revoke(Guid sessionId);

        /// <summary>
        /// Ends every session of the identity the request is served for except the current
        /// one, signing the account out everywhere else.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>The current instance for method chaining.</returns>
        IIdentitySessionManager RevokeOthers(IRequest request);
    }
}
