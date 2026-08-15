using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Manages the devices and browsers that are currently signed in with an identity, as
    /// listed on the profile's "active sessions" page.
    /// </summary>
    /// <remarks>
    /// The session marked <see cref="IdentitySession.Current"/> is the one the page is being
    /// served to; it is never ended from here, so a user cannot lock themselves out of the
    /// very page they are looking at.
    /// </remarks>
    public sealed class IdentitySessionManager : IIdentitySessionManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when a session is ended.
        /// </summary>
        public event EventHandler<IdentitySession> IdentitySessionRemoved;

        /// <summary>
        /// Initializes a new instance of the class. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private IdentitySessionManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns the sessions of the identity the request is served for, the current device
        /// first.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>An enumerable collection of sessions (possibly empty).</returns>
        public IEnumerable<IdentitySession> GetSessions(IRequest request)
        {
            return GetSessions(CoreHub.SessionManager.GetCurrentIdentityId(request));
        }

        /// <summary>
        /// Returns the sessions of the given identity, the current device first.
        /// </summary>
        /// <param name="ownerId">The signed-in identity.</param>
        /// <returns>An enumerable collection of sessions (possibly empty).</returns>
        public IEnumerable<IdentitySession> GetSessions(Guid ownerId)
        {
            return ModelHub.GetIdentitySessions(ownerId);
        }

        /// <summary>
        /// Returns a session by its id.
        /// </summary>
        /// <param name="sessionId">The id of the session.</param>
        /// <returns>The session, or <see langword="null"/> when no such session exists.</returns>
        public IdentitySession GetSession(Guid sessionId)
        {
            return ModelHub.GetIdentitySession(sessionId);
        }

        /// <summary>
        /// Ends the session with the given id, signing the account out on that device. The
        /// session the request itself is served on is left untouched.
        /// </summary>
        /// <param name="sessionId">The id of the session to end.</param>
        /// <returns>The current instance for method chaining.</returns>
        public IIdentitySessionManager Revoke(Guid sessionId)
        {
            var session = GetSession(sessionId);

            if (session is null || session.Current)
            {
                return this;
            }

            ModelHub.RemoveIdentitySession(sessionId);

            IdentitySessionRemoved?.Invoke(this, session);

            CoreHub.AddNotification("kleenestar.core:notification.title.deleted", "kleenestar.core:notification.session.revoked", 5000);

            return this;
        }

        /// <summary>
        /// Ends every session of the identity the request is served for except the current
        /// one, signing the account out everywhere else.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>The current instance for method chaining.</returns>
        public IIdentitySessionManager RevokeOthers(IRequest request)
        {
            var ownerId = CoreHub.SessionManager.GetCurrentIdentityId(request);

            if (ownerId == Guid.Empty || ModelHub.RemoveOtherIdentitySessions(ownerId) == 0)
            {
                return this;
            }

            IdentitySessionRemoved?.Invoke(this, null);

            CoreHub.AddNotification("kleenestar.core:notification.title.deleted", "kleenestar.core:notification.session.revokedall", 5000);

            return this;
        }

        /// <summary>
        /// Release of unmanaged resources reserved during use.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
