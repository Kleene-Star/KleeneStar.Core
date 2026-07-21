using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing objects, including adding, retrieving, and removing, as well as
    /// handling object-related events.
    /// </summary>
    public interface IObjectManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when an object is added.
        /// </summary>
        event System.EventHandler<Object> ObjectAdded;

        /// <summary>
        /// An event that fires when an object is udpated.
        /// </summary>
        event System.EventHandler<Object> ObjectUpdated;

        /// <summary>
        /// An event that fires when an object is removed.
        /// </summary>
        event System.EventHandler<Object> ObjectRemoved;

        /// <summary>
        /// Returns a object based on its id.
        /// </summary>
        /// <param name="objectId">The id of the object.</param>
        /// <returns>The object.</returns>
        Object GetObject(System.Guid objectId);

        /// <summary>
        /// Retrieves the first object that matches the specified key, using a 
        /// case-insensitive comparison.
        /// </summary>
        /// <param name="key">
        /// The key used to identify the object to retrieve. Must not be null, empty, or 
        /// consist only of whitespace.
        /// </param>
        /// <returns>
        /// The object associated with the specified key, or null if no matching object is 
        /// found or if the key is invalid.
        /// </returns>
        Object GetObjectByKey(string key);

        /// <summary>
        /// Returns a object based on its key.
        /// </summary>
        /// <param name="objectKey">The key of the object.</param>
        /// <returns>The object.</returns>
        Object GetObjectByKey(ObjectKeyParameter objectKey);

        /// <summary>
        /// Retrieves a collection of objects that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned objects. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of objects that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Object> GetObjects(IQuery<Object> query);

        /// <summary>
        /// Retrieves a collection of bojects that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned objects. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of objects that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Object> GetObjects(IQuery<Object> query, IQueryContext context);

        /// <summary>
        /// Returns the active objects the supplied identity has most recently opened, newest
        /// first, capped at <paramref name="count"/>. Backs the "recently used" section of the
        /// object dropdown in the application header.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="count">The maximum number of objects to return.</param>
        /// <returns>The recently opened objects, newest first. The collection may be empty.</returns>
        IReadOnlyList<Object> GetRecentObjects(System.Guid ownerId, int count);

        /// <summary>
        /// Returns the active objects of the supplied kind the supplied identity has most
        /// recently opened, newest first, capped at <paramref name="count"/>. Backs the
        /// "recently used" section of a per-kind dropdown in the application header.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="count">The maximum number of objects to return.</param>
        /// <param name="kind">
        /// The kind key to filter by. Null, empty, or whitespace returns objects of every
        /// kind (equivalent to <see cref="GetRecentObjects(System.Guid, int)"/>).
        /// </param>
        /// <returns>The recently opened objects of the kind, newest first. The collection may be empty.</returns>
        IReadOnlyList<Object> GetRecentObjects(System.Guid ownerId, int count, string kind);

        /// <summary>
        /// Records that the supplied identity has just opened the supplied object by advancing the
        /// visit's last-visited timestamp (inserting the visit when needed). The mutation is
        /// deliberately quiet because it fires on every object detail page load. Returns
        /// <see langword="null"/> when the owner or object does not exist.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <returns>The persisted visit, or <see langword="null"/>.</returns>
        ObjectVisit RecordVisit(System.Guid ownerId, System.Guid objectId);

        /// <summary>
        /// Returns the active objects the supplied identity has starred, ordered by key.
        /// Backs the "starred" quickfilter of the issues overview.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <returns>The starred objects. The collection may be empty.</returns>
        IReadOnlyList<Object> GetFavoriteObjects(System.Guid ownerId);

        /// <summary>
        /// Returns whether the supplied identity has starred the supplied object.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <returns><see langword="true"/> when the object is starred by the identity.</returns>
        bool IsFavorite(System.Guid ownerId, System.Guid objectId);

        /// <summary>
        /// Sets the starred state of the supplied object for the supplied identity, inserting
        /// or updating the backing visit row. Returns <see langword="null"/> when the owner or
        /// object does not exist.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="favorite">The new starred state.</param>
        /// <returns>The persisted visit row, or <see langword="null"/>.</returns>
        ObjectVisit SetFavorite(System.Guid ownerId, System.Guid objectId, bool favorite);

        /// <summary>
        /// Adds a object to the manager.
        /// </summary>
        /// <param name="objectEntity">The object to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IObjectManager Add(Object objectEntity);

        /// <summary>
        /// Update a object to the manager.
        /// </summary>
        /// <param name="objectEntity">The object to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IObjectManager Update(Object objectEntity);

        /// <summary>
        /// Removes the specified object from the manager.
        /// </summary>
        /// <remarks>This method removes the specified object from the manager. If the object does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="objectId">The object id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IObjectManager Remove(System.Guid objectId);

        /// <summary>
        /// Returns the parent of the specified object, or <c>null</c> when none is set.
        /// </summary>
        /// <param name="objectId">The id of the child object.</param>
        Object GetParent(System.Guid objectId);

        /// <summary>
        /// Returns the immediate children of the specified object.
        /// </summary>
        /// <param name="objectId">The id of the parent object.</param>
        IEnumerable<Object> GetChildren(System.Guid objectId);

        /// <summary>
        /// Returns the siblings of the specified object: every other object inside the
        /// same workspace and class. The supplied object itself is excluded from the
        /// result.
        /// </summary>
        /// <param name="objectId">The id of the reference object.</param>
        IEnumerable<Object> GetSiblings(System.Guid objectId);

        /// <summary>
        /// Returns the ancestor chain of the specified object, nearest first (parent,
        /// grandparent, …, root). The chain stops defensively when a cycle is detected
        /// in persisted data.
        /// </summary>
        /// <param name="objectId">The id of the object whose ancestors are resolved.</param>
        /// <returns>The ancestors, nearest first. The collection may be empty.</returns>
        IEnumerable<Object> GetAncestors(System.Guid objectId);

        /// <summary>
        /// Returns every descendant of the specified object (children, grandchildren, …)
        /// in breadth-first order. The traversal stops defensively when a cycle is
        /// detected in persisted data.
        /// </summary>
        /// <param name="objectId">The id of the object whose subtree is resolved.</param>
        /// <returns>The descendants in breadth-first order. The collection may be empty.</returns>
        IEnumerable<Object> GetDescendants(System.Guid objectId);

        /// <summary>
        /// Sets or clears the parent of the specified object after validating the
        /// hierarchy rules: the parent must exist, must not be the object itself, must
        /// not be one of the object's descendants (no cycles), must live in the same
        /// workspace, and — when the parent's class declares
        /// <see cref="Class.AllowedChildren"/> — the object's class must be allowed.
        /// </summary>
        /// <param name="objectId">The id of the object whose parent is set.</param>
        /// <param name="parentId">The id of the new parent, or <c>null</c> to detach.</param>
        /// <returns>
        /// The updated object, or <c>null</c> when no object with the supplied id exists.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when one of the hierarchy rules is violated.
        /// </exception>
        Object SetParent(System.Guid objectId, System.Guid? parentId);
    }
}
