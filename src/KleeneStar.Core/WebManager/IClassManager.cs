using System;
using System.Collections.Generic;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
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
    public interface IClassManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when an class is added.
        /// </summary>
        event EventHandler<Class> ClassAdded;

        /// <summary>
        /// An event that fires when an class is udpated.
        /// </summary>
        event EventHandler<Class> ClassUpdated;

        /// <summary>
        /// An event that fires when an class is removed.
        /// </summary>
        event EventHandler<Class> ClassRemoved;

        /// <summary>
        /// Returns a class based on its id.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>The class.</returns>
        Class GetClass(Guid classId);

        /// <summary>
        /// Returns a class based on its id.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>The class.</returns>
        Class GetClass(ClassIdParameter classId);

        /// <summary>
        /// Retrieves a collection of classes that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned classes. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of classes that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Class> GetClasses(IQuery<Class> query);

        /// <summary>
        /// Retrieves a collection of classes that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned classes. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of classes that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Class> GetClasses(IQuery<Class> query, IQueryContext context);

        /// <summary>
        /// Adds a class to the manager.
        /// </summary>
        /// <param name="classEntity">The class to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IClassManager AddClass(Class classEntity);

        /// <summary>
        /// Update a class to the manager.
        /// </summary>
        /// <param name="classEntity">The class to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IClassManager UpdateClass(Class classEntity);

        /// <summary>
        /// Removes the specified class from the manager.
        /// </summary>
        /// <remarks>This method removes the specified class from the manager. If the class does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="classId">The class id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IClassManager RemoveClass(Guid classId);
    }
}
