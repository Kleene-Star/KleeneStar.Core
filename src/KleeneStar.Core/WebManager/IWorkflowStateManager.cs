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
    public interface IWorkflowStateManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when an workflow state is added.
        /// </summary>
        event EventHandler<WorkflowState> StateAdded;

        /// <summary>
        /// An event that fires when an workflow state is udpated.
        /// </summary>
        event EventHandler<WorkflowState> StateUpdated;

        /// <summary>
        /// An event that fires when an workflow state is removed.
        /// </summary>
        event EventHandler<WorkflowState> StateRemoved;

        /// <summary>
        /// Returns a workflow state based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow state.</param>
        /// <returns>The workflow state.</returns>
        WorkflowState GetState(Guid workflowId);

        /// <summary>
        /// Returns a workflow state based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow state.</param>
        /// <returns>The workflow state.</returns>
        WorkflowState GetState(WorkflowStateIdParameter workflowId);

        /// <summary>
        /// Retrieves a collection of workflow states that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>
        /// An enumerable collection of workflow states that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<WorkflowState> GetStates(ClassIdParameter classId);

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
        IEnumerable<WorkflowState> GetStates(IQuery<WorkflowState> query);

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
        IEnumerable<WorkflowState> GetStates(IQuery<WorkflowState> query, IQueryContext context);

        /// <summary>
        /// Adds a workflow state to the manager.
        /// </summary>
        /// <param name="stateEntity">The workflow state to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IWorkflowStateManager Add(WorkflowState stateEntity);

        /// <summary>
        /// Update a workflow state to the manager.
        /// </summary>
        /// <param name="stateEntity">The workflow state to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IWorkflowStateManager Update(WorkflowState stateEntity);

        /// <summary>
        /// Removes the specified workflow state from the manager.
        /// </summary>
        /// <remarks>This method removes the specified workflow state from the manager. If the workflow state does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="stateId">The workflow state id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IWorkflowStateManager Remove(Guid stateId);
    }
}
