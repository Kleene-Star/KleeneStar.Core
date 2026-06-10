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
