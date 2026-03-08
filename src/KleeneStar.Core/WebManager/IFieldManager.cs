using System;
using System.Collections.Generic;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing fields, including adding, retrieving, and removing, as well as
    /// handling field-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing fields and events for tracking changes 
    /// to the field collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public interface IFieldManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when an field is added.
        /// </summary>
        event EventHandler<Field> FieldAdded;

        /// <summary>
        /// An event that fires when an field is udpated.
        /// </summary>
        event EventHandler<Field> FieldUpdated;

        /// <summary>
        /// An event that fires when an field is removed.
        /// </summary>
        event EventHandler<Field> FieldRemoved;

        /// <summary>
        /// Returns a field based on its id.
        /// </summary>
        /// <param name="fieldId">The id of the field.</param>
        /// <returns>The field.</returns>
        Field GetField(Guid fieldId);

        /// <summary>
        /// Returns a field based on its id.
        /// </summary>
        /// <param name="fieldId">The id of the field.</param>
        /// <returns>The field.</returns>
        Field GetField(FieldIdParameter fieldId);

        /// <summary>
        /// Retrieves a collection of fields that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>
        /// An enumerable collection of fields that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Field> GetFields(ClassIdParameter classId);

        /// <summary>
        /// Retrieves a collection of fields that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned fields. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of fields that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Field> GetFields(IQuery<Field> query);

        /// <summary>
        /// Retrieves a collection of fields that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned fields. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of fields that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Field> GetFields(IQuery<Field> query, IQueryContext context);

        /// <summary>
        /// Adds a field to the manager.
        /// </summary>
        /// <param name="fieldEntity">The field to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IFieldManager AddField(Field fieldEntity);

        /// <summary>
        /// Update a field to the manager.
        /// </summary>
        /// <param name="fieldEntity">The field to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IFieldManager UpdateField(Field fieldEntity);

        /// <summary>
        /// Removes the specified field from the manager.
        /// </summary>
        /// <remarks>This method removes the specified field from the manager. If the field does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="fieldId">The field id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IFieldManager RemoveField(Guid fieldId);
    }
}
