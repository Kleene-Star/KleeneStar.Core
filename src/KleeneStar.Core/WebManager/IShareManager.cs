using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing share relationships between objects and
    /// identities. Sharing grants the linked identity read/comment access to the
    /// object (e.g. a portal issue) without making it the requester.
    /// </summary>
    public interface IShareManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when a share has been granted.
        /// </summary>
        event EventHandler<ObjectShare> ShareAdded;

        /// <summary>
        /// An event that fires when a share has been revoked.
        /// </summary>
        event EventHandler<ObjectShare> ShareRemoved;

        /// <summary>
        /// Returns every share attached to the object addressed by the supplied
        /// URL-bound object-key parameter, in chronological order (oldest first).
        /// </summary>
        /// <param name="objectKey">The object-key parameter.</param>
        /// <returns>The shares attached to the object. The collection may be empty.</returns>
        IEnumerable<ObjectShare> GetShares(ObjectKeyParameter objectKey);

        /// <summary>
        /// Returns every share attached to the object with the supplied id, in
        /// chronological order (oldest first).
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The shares attached to the object. The collection may be empty.</returns>
        IEnumerable<ObjectShare> GetShares(Guid objectId);

        /// <summary>
        /// Returns the shares that satisfy the supplied query.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching shares.</returns>
        IEnumerable<ObjectShare> GetShares(IQuery<ObjectShare> query);

        /// <summary>
        /// Returns the shares that satisfy the supplied query, executed inside the
        /// supplied query context.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching shares.</returns>
        IEnumerable<ObjectShare> GetShares(IQuery<ObjectShare> query, IQueryContext context);

        /// <summary>
        /// Grants the supplied identity access to the supplied object. When the
        /// identity already holds a share on the object, the existing row is
        /// returned. Returns <see langword="null"/> when either side does not exist.
        /// </summary>
        /// <param name="objectId">The id of the shared object.</param>
        /// <param name="identityId">The id of the identity the object is shared with.</param>
        /// <returns>The persisted share relationship, or <see langword="null"/>.</returns>
        ObjectShare Add(Guid objectId, Guid identityId);

        /// <summary>
        /// Revokes the share between the supplied object and identity.
        /// </summary>
        /// <param name="objectId">The id of the shared object.</param>
        /// <param name="identityId">The id of the identity whose share is revoked.</param>
        /// <returns><see langword="true"/> when a row existed and was removed.</returns>
        bool Remove(Guid objectId, Guid identityId);
    }
}
