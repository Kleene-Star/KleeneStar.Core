using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing workflow states, including adding, retrieving, and removing, as well as
    /// handling state-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing states and events for tracking changes 
    /// to the state collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public interface IStatusManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when an workflow state is added.
        /// </summary>
        event EventHandler<Status> StatusAdded;

        /// <summary>
        /// An event that fires when an workflow state is udpated.
        /// </summary>
        event EventHandler<Status> StatusUpdated;

        /// <summary>
        /// An event that fires when an workflow state is removed.
        /// </summary>
        event EventHandler<Status> StatusRemoved;

        /// <summary>
        /// Retrieves a collection of status categories that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// The query used to filter and select status categories. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of status categories that satisfy the query conditions. The 
        /// collection is empty if no status categories match the query.
        /// </returns>
        IEnumerable<StatusCategory> GetStatusCategories(IQuery<StatusCategory> query);

        /// <summary>
        /// Retrieves a collection of status categories that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// The query used to filter and select status categories. Cannot be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of status categories that satisfy the query conditions. The 
        /// collection is empty if no status categories match the query.
        /// </returns>
        IEnumerable<StatusCategory> GetStatusCategories(IQuery<StatusCategory> query, IQueryContext context);

        /// <summary>
        /// Returns a workflow state based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow state.</param>
        /// <returns>The workflow state.</returns>
        Status GetStatus(Guid workflowId);

        /// <summary>
        /// Returns a workflow state based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow state.</param>
        /// <returns>The workflow state.</returns>
        Status GetStatus(WorkflowStateIdParameter workflowId);

        /// <summary>
        /// Retrieves a collection of workflow states that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>
        /// An enumerable collection of workflow states that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Status> GetStatuses(ClassIdParameter classId);

        /// <summary>
        /// Retrieves a collection of workflow states that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned workflow states. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of workflow states that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Status> GetStatuses(IQuery<Status> query);

        /// <summary>
        /// Retrieves a collection of workflow states that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned workflow states. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of workflow states that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Status> GetStatuses(IQuery<Status> query, IQueryContext context);

        /// <summary>
        /// Adds a workflow state to the manager.
        /// </summary>
        /// <param name="stateEntity">The workflow state to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IStatusManager Add(Status stateEntity);

        /// <summary>
        /// Update a workflow state to the manager.
        /// </summary>
        /// <param name="stateEntity">The workflow state to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IStatusManager Update(Status stateEntity);

        /// <summary>
        /// Removes the specified workflow state from the manager.
        /// </summary>
        /// <remarks>This method removes the specified workflow state from the manager. If the workflow state does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="stateId">The workflow state id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IStatusManager Remove(Guid stateId);
    }
}
