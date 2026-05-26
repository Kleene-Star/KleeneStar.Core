using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing watcher relationships
    /// between identity and object.
    /// </summary>
    public interface IWatcherManager : IComponentManager
    {
        /// <summary>
        /// Raised when an identity has started watching an object.
        /// </summary>
        event EventHandler<ObjectWatcher> WatcherAdded;

        /// <summary>
        /// Raised when an identity has stopped watching an object.
        /// </summary>
        event EventHandler<ObjectWatcher> WatcherRemoved;

        /// <summary>
        /// Returns every watch relationship attached to the supplied object (parameter
        /// form), in chronological order (oldest first).
        /// </summary>
        /// <param name="objectKey">The object-key parameter parsed from the URL path.</param>
        /// <returns>The watchers attached to the object. The collection may be empty.</returns>
        IEnumerable<ObjectWatcher> GetWatchers(ObjectKeyParameter objectKey);

        /// <summary>
        /// Returns every watch relationship attached to the object with the supplied
        /// id, in chronological order (oldest first).
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The watchers attached to the object. The collection may be empty.</returns>
        IEnumerable<ObjectWatcher> GetWatchers(Guid objectId);

        /// <summary>
        /// Returns the watch relationships that satisfy the supplied query.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching watchers.</returns>
        IEnumerable<ObjectWatcher> GetWatchers(IQuery<ObjectWatcher> query);

        /// <summary>
        /// Returns the watch relationships that satisfy the supplied query, executed
        /// inside the supplied <see cref="IQueryContext"/>.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching watchers.</returns>
        IEnumerable<ObjectWatcher> GetWatchers(IQuery<ObjectWatcher> query, IQueryContext context);

        /// <summary>
        /// Adds a watch relationship between the supplied object and identity. Returns
        /// the persisted entity. When the identity is already watching the object, the
        /// existing row is returned and no change is made.
        /// </summary>
        /// <param name="objectId">The id of the object being watched.</param>
        /// <param name="identityId">The id of the watching identity.</param>
        /// <returns>The persisted watch relationship, or <see langword="null"/> when
        /// the object or identity does not exist.</returns>
        ObjectWatcher Add(Guid objectId, Guid identityId);

        /// <summary>
        /// Removes the watch relationship between the supplied object and identity.
        /// </summary>
        /// <param name="objectId">The id of the watched object.</param>
        /// <param name="identityId">The id of the watching identity.</param>
        /// <returns><see langword="true"/> when a row existed and was removed.</returns>
        bool Remove(Guid objectId, Guid identityId);
    }
}
