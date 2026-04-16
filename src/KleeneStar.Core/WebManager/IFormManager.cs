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
        IFormManager Add(Form formEntity);

        /// <summary>
        /// Update a form to the manager.
        /// </summary>
        /// <param name="formEntity">The form to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IFormManager Update(Form formEntity);

        /// <summary>
        /// Removes the specified form from the manager.
        /// </summary>
        /// <remarks>This method removes the specified form from the manager. If the form does
        /// not exist in the manager, no action is taken. Standard forms cannot be removed.</remarks>
        /// <param name="formId">The form id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IFormManager Remove(Guid formId);

        /// <summary>
        /// Determines whether the specified form is the standard form of its class.
        /// </summary>
        /// <remarks>
        /// A standard form is the automatically created, non-deletable form that provides
        /// the three predefined views (new, edit, view) for a class. Each class has exactly
        /// one standard form. Standard forms are identified by the reserved name defined in
        /// <see cref="FormManager.StandardFormName"/>.
        /// </remarks>
        /// <param name="formId">The unique identifier of the form to check.</param>
        /// <returns>
        /// <c>true</c> if the form is the standard form for its class; otherwise, <c>false</c>.
        /// </returns>
        bool IsStandardForm(Guid formId);

        /// <summary>
        /// Creates the standard form for the specified class.
        /// </summary>
        /// <remarks>
        /// The standard form is the automatically created, non-deletable form that provides
        /// the three predefined views (new, edit, view) for a class. This method should be
        /// called when a new class is created to ensure every class has exactly one standard form.
        /// If a standard form already exists for the class, no action is taken.
        /// </remarks>
        /// <param name="classId">The unique identifier of the class for which to create the standard form.</param>
        /// <returns>The created standard form, or the existing one if it already exists.</returns>
        Form CreateStandardForm(Guid classId);
    }
}
