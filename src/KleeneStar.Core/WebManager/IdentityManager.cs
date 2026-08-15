using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Manages identity entities within the application.
    /// </summary>
    public sealed class IdentityManager : IIdentityManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when an identity is added.
        /// </summary>
        public event EventHandler<Identity> IdentityAdded;

        /// <summary>
        /// An event that fires when an identity is updated.
        /// </summary>
        public event EventHandler<Identity> IdentityUpdated;

        /// <summary>
        /// An event that fires when an identity is removed.
        /// </summary>
        public event EventHandler<Identity> IdentityRemoved;

        /// <summary>
        /// Returns the collection of names that are reserved and cannot be used for identities.
        /// </summary>
        public static IEnumerable<string> ReservedIdentityNames =>
        [
            "default", "admin", "system", "assets", "api", "identity",
            "identities", "icons", "setting"
        ];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private IdentityManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns an identity based on its id.
        /// </summary>
        /// <param name="identityId">The id of the identity.</param>
        /// <returns>The identity.</returns>
        public Identity GetIdentity(Guid identityId)
        {
            var query = new Query<Identity>()
                .Where(x => x.Id == identityId)
                .WithPaging(0, 1);

            return ModelHub.GetIdentities(query)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns an identity based on its id parameter.
        /// </summary>
        /// <param name="identityId">The id parameter of the identity.</param>
        /// <returns>The identity.</returns>
        public Identity GetIdentity(IdentityIdParameter identityId)
        {
            var guid = Guid.TryParse(identityId.Value, out Guid id) ? id : Guid.Empty;

            return GetIdentity(guid);
        }

        /// <summary>
        /// Returns the identity the given request is served for — the account whose profile
        /// settings the profile pages read and write.
        /// </summary>
        /// <remarks>
        /// Which identity that is comes from <see cref="ISessionManager.GetCurrentIdentityId"/>,
        /// so the profile pages follow the authenticated user as soon as WebExpress exposes it
        /// on the request.
        /// </remarks>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>
        /// The identity of the caller, or <see langword="null"/> when no identity can be resolved.
        /// </returns>
        public Identity GetCurrentIdentity(IRequest request)
        {
            var identityId = CoreHub.SessionManager.GetCurrentIdentityId(request);

            return identityId == Guid.Empty ? null : GetIdentity(identityId);
        }

        /// <summary>
        /// Retrieves a collection of identities that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>An enumerable collection of identities.</returns>
        public IEnumerable<Identity> GetIdentities(IQuery<Identity> query)
        {
            return ModelHub.GetIdentities(query);
        }

        /// <summary>
        /// Retrieves a collection of identities that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The query context.</param>
        /// <returns>An enumerable collection of identities.</returns>
        public IEnumerable<Identity> GetIdentities(IQuery<Identity> query, IQueryContext context)
        {
            return ModelHub.GetIdentities(query, context as KleeneStarDbContext);
        }

        /// <summary>
        /// Adds an identity.
        /// </summary>
        /// <param name="identityEntity">The identity to add.</param>
        /// <returns>The current instance for method chaining.</returns>
        public IIdentityManager Add(Identity identityEntity)
        {
            ArgumentNullException.ThrowIfNull(identityEntity);

            ModelHub.Add(identityEntity);

            IdentityAdded?.Invoke(this, identityEntity);

            CoreHub.AddNotification("kleenestar.core:notification.title.created", "kleenestar.core:notification.identity.created", 5000);

            return this;
        }

        /// <summary>
        /// Updates an identity.
        /// </summary>
        /// <param name="identityEntity">The identity to update.</param>
        /// <returns>The current instance for method chaining.</returns>
        public IIdentityManager Update(Identity identityEntity)
        {
            ArgumentNullException.ThrowIfNull(identityEntity);

            ModelHub.Update(identityEntity);

            IdentityUpdated?.Invoke(this, identityEntity);

            CoreHub.AddNotification("kleenestar.core:notification.title.updated", "kleenestar.core:notification.identity.updated", 5000);

            return this;
        }

        /// <summary>
        /// Removes an identity.
        /// </summary>
        /// <param name="identityId">The identity id to remove.</param>
        /// <returns>The current instance for method chaining.</returns>
        public IIdentityManager Remove(Guid identityId)
        {
            var identityEntry = GetIdentity(identityId);

            if (identityEntry is not null)
            {
                ModelHub.Remove(identityEntry);
                IdentityRemoved?.Invoke(this, identityEntry);
            }

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
