using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing classes, including adding, retrieving, and removing, as well as
    /// handling class-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing classes and events for tracking changes 
    /// to the class collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public sealed class ObjectManager : IObjectManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when an object is added.
        /// </summary>
        public event EventHandler<Model.Entities.Object> ObjectAdded;

        /// <summary>
        /// An event that fires when an object is udpated.
        /// </summary>
        public event EventHandler<Model.Entities.Object> ObjectUpdated;

        /// <summary>
        /// An event that fires when an object is removed.
        /// </summary>
        public event EventHandler<Model.Entities.Object> ObjectRemoved;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private ObjectManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns a object based on its id.
        /// </summary>
        /// <param name="objectId">The id of the object.</param>
        /// <returns>The object.</returns>
        public Model.Entities.Object GetObject(Guid objectId)
        {
            var query = new Query<Model.Entities.Object>()
                .Where(x => x.Id == objectId)
                .WithPaging(0, 1);

            return ModelHub.GetObjects(query)
                .FirstOrDefault();
        }

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
        public Model.Entities.Object GetObjectByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            var query = new Query<Model.Entities.Object>()
                .WhereEqualsIgnoreCase(x => x.Key, key)
                .WithPaging(0, 1);

            return ModelHub.GetObjects(query)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns a object based on its key.
        /// </summary>
        /// <param name="objectKey">The key of the object.</param>
        /// <returns>The object.</returns>
        public Model.Entities.Object GetObjectByKey(ObjectKeyParameter objectKey)
        {
            var key = objectKey?.Value;

            return GetObjectByKey(key);
        }


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
        public IEnumerable<Model.Entities.Object> GetObjects(IQuery<Model.Entities.Object> query)
        {
            return ModelHub.GetObjects(query);
        }

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
        public IEnumerable<Model.Entities.Object> GetObjects(IQuery<Model.Entities.Object> query, IQueryContext context)
        {
            return ModelHub.GetObjects(query, context as KleeneStarDbContext);
        }

        /// <summary>
        /// Adds a object to the manager.
        /// </summary>
        /// <param name="objectEntity">The object to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IObjectManager Add(Model.Entities.Object objectEntity)
        {
            ArgumentNullException.ThrowIfNull(objectEntity);

            ModelHub.Add(objectEntity);

            ObjectAdded?.Invoke(this, objectEntity);

            // create notification
            CoreHub.AddNotification("Create", "success", 5000);

            return this;
        }

        /// <summary>
        /// Update a object to the manager.
        /// </summary>
        /// <param name="objectEntity">The object to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IObjectManager Update(Model.Entities.Object objectEntity)
        {
            ArgumentNullException.ThrowIfNull(objectEntity);

            ModelHub.Update(objectEntity);

            ObjectUpdated?.Invoke(this, objectEntity);

            // create notification
            CoreHub.AddNotification("Update", "success", 5000);

            return this;
        }

        /// <summary>
        /// Removes the specified object from the manager.
        /// </summary>
        /// <remarks>This method removes the specified object from the manager. If the object does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="objectId">The object id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IObjectManager Remove(Guid objectId)
        {
            var objectEntry = GetObject(objectId);

            if (objectEntry is not null)
            {
                ModelHub.Remove(objectEntry);
                ObjectRemoved?.Invoke(this, objectEntry);
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
