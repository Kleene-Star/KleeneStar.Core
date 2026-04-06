using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing forms, including adding, retrieving, and removing, as well as
    /// handling form-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing forms and events for tracking changes 
    /// to the form collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public interface IFormManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when an form is added.
        /// </summary>
        event EventHandler<Form> FormAdded;

        /// <summary>
        /// An event that fires when an form is udpated.
        /// </summary>
        event EventHandler<Form> FormUpdated;

        /// <summary>
        /// An event that fires when an form is removed.
        /// </summary>
        event EventHandler<Form> FormRemoved;

        /// <summary>
        /// Returns a form based on its id.
        /// </summary>
        /// <param name="formId">The id of the form.</param>
        /// <returns>The form.</returns>
        Form GetForm(Guid formId);

        /// <summary>
        /// Returns a form based on its id.
        /// </summary>
        /// <param name="formId">The id of the form.</param>
        /// <returns>The form.</returns>
        Form GetForm(FormIdParameter formId);

        /// <summary>
        /// Retrieves a collection of forms that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>
        /// An enumerable collection of forms that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Form> GetForms(ClassIdParameter classId);

        /// <summary>
        /// Retrieves a collection of forms that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned forms. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of forms that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Form> GetForms(IQuery<Form> query);

        /// <summary>
        /// Retrieves a collection of forms that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned forms. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of forms that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Form> GetForms(IQuery<Form> query, IQueryContext context);

        /// <summary>
        /// Adds a form to the manager.
        /// </summary>
        /// <param name="formEntity">The form to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IFormManager AddForm(Form formEntity);

        /// <summary>
        /// Update a form to the manager.
        /// </summary>
        /// <param name="formEntity">The form to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IFormManager UpdateForm(Form formEntity);

        /// <summary>
        /// Removes the specified form from the manager.
        /// </summary>
        /// <remarks>This method removes the specified form from the manager. If the form does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="formId">The form id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IFormManager RemoveForm(Guid formId);
    }
}
