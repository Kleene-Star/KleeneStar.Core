using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Manages the personal access tokens an identity created for API access and integrations.
    /// </summary>
    /// <remarks>
    /// The secret of a token exists exactly once: <see cref="Create"/> hands it to the caller
    /// and stores only its hash plus the leading, non-secret prefix. Everything the token list
    /// shows afterwards — name, prefix, scopes, dates — is metadata, never the credential.
    /// </remarks>
    public sealed class AccessTokenManager : IAccessTokenManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// The prefix every token secret carries so it can be recognized in a log or a
        /// configuration file as a KleeneStar credential.
        /// </summary>
        private const string TokenPrefix = "kls_";

        /// <summary>
        /// The number of characters of the secret that are kept in clear as the token's
        /// recognizable prefix.
        /// </summary>
        private const int VisibleLength = 8;

        /// <summary>
        /// An event that fires when a token is created.
        /// </summary>
        public event EventHandler<AccessToken> AccessTokenAdded;

        /// <summary>
        /// An event that fires when a token is updated or revoked.
        /// </summary>
        public event EventHandler<AccessToken> AccessTokenUpdated;

        /// <summary>
        /// An event that fires when a token is deleted.
        /// </summary>
        public event EventHandler<AccessToken> AccessTokenRemoved;

        /// <summary>
        /// Initializes a new instance of the class. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private AccessTokenManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns the tokens of the identity the request is served for, newest first.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>An enumerable collection of tokens (possibly empty).</returns>
        public IEnumerable<AccessToken> GetAccessTokens(IRequest request)
        {
            return GetAccessTokens(CoreHub.SessionManager.GetCurrentIdentityId(request));
        }

        /// <summary>
        /// Returns the tokens owned by the given identity, newest first.
        /// </summary>
        /// <param name="ownerId">The identity that owns the tokens.</param>
        /// <returns>An enumerable collection of tokens (possibly empty).</returns>
        public IEnumerable<AccessToken> GetAccessTokens(Guid ownerId)
        {
            return ModelHub.GetAccessTokens(ownerId);
        }

        /// <summary>
        /// Returns a token by its id.
        /// </summary>
        /// <param name="tokenId">The id of the token.</param>
        /// <returns>The token, or <see langword="null"/> when no such token exists.</returns>
        public AccessToken GetAccessToken(Guid tokenId)
        {
            return ModelHub.GetAccessToken(tokenId);
        }

        /// <summary>
        /// Creates a token for the identity the request is served for and returns the secret
        /// exactly once — it is stored hashed and can never be read again.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="name">The label the owner gave the token.</param>
        /// <param name="scopes">The scopes the token grants, separated by spaces.</param>
        /// <param name="lifetime">
        /// How long the token is valid, or <see langword="null"/> when it never expires.
        /// </param>
        /// <param name="secret">
        /// When the method returns, contains the token secret to hand to the owner.
        /// </param>
        /// <returns>The created token.</returns>
        public AccessToken Create(IRequest request, string name, string scopes, TimeSpan? lifetime, out string secret)
        {
            var ownerId = CoreHub.SessionManager.GetCurrentIdentityId(request);

            if (ownerId == Guid.Empty)
            {
                secret = null;
                return null;
            }

            secret = TokenPrefix + RandomNumberGenerator.GetHexString(48, lowercase: false);

            var token = new AccessToken
            {
                OwnerId = ownerId,
                Name = string.IsNullOrWhiteSpace(name) ? "Token" : name.Trim(),
                Prefix = secret[..(TokenPrefix.Length + VisibleLength)],
                TokenHash = Hash(secret),
                Scopes = scopes,
                Created = DateTime.UtcNow,
                Expires = lifetime.HasValue ? DateTime.UtcNow.Add(lifetime.Value) : null
            };

            ModelHub.Add(token);

            AccessTokenAdded?.Invoke(this, token);

            CoreHub.AddNotification("kleenestar.core:notification.title.created", "kleenestar.core:notification.token.created", 5000);

            return token;
        }

        /// <summary>
        /// Updates a token.
        /// </summary>
        /// <param name="token">The token to update.</param>
        /// <returns>The current instance for method chaining.</returns>
        public IAccessTokenManager Update(AccessToken token)
        {
            ArgumentNullException.ThrowIfNull(token);

            ModelHub.Update(token);

            AccessTokenUpdated?.Invoke(this, token);

            CoreHub.AddNotification("kleenestar.core:notification.title.updated", "kleenestar.core:notification.token.updated", 5000);

            return this;
        }

        /// <summary>
        /// Revokes the token with the given id, so it stops authenticating requests without
        /// disappearing from the owner's audit trail.
        /// </summary>
        /// <param name="tokenId">The id of the token to revoke.</param>
        /// <returns>The current instance for method chaining.</returns>
        public IAccessTokenManager Revoke(Guid tokenId)
        {
            var token = GetAccessToken(tokenId);

            if (token is null || token.Revoked)
            {
                return this;
            }

            token.Revoked = true;
            ModelHub.Update(token);

            AccessTokenUpdated?.Invoke(this, token);

            CoreHub.AddNotification("kleenestar.core:notification.title.updated", "kleenestar.core:notification.token.revoked", 5000);

            return this;
        }

        /// <summary>
        /// Deletes the token with the given id.
        /// </summary>
        /// <param name="tokenId">The id of the token to delete.</param>
        /// <returns>The current instance for method chaining.</returns>
        public IAccessTokenManager Remove(Guid tokenId)
        {
            var token = GetAccessToken(tokenId);

            if (token is null)
            {
                return this;
            }

            ModelHub.RemoveAccessToken(tokenId);

            AccessTokenRemoved?.Invoke(this, token);

            CoreHub.AddNotification("kleenestar.core:notification.title.deleted", "kleenestar.core:notification.token.deleted", 5000);

            return this;
        }

        /// <summary>
        /// Returns the hash under which the given secret is stored.
        /// </summary>
        /// <param name="secret">The token secret handed to the owner.</param>
        /// <returns>The hexadecimal SHA-256 hash of the secret.</returns>
        private static string Hash(string secret)
        {
            return Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(secret)));
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
