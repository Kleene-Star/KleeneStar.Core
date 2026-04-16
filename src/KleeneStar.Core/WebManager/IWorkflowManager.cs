using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing workflows, including adding, retrieving, and removing, as well as
    /// handling workflow-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing workflows and events for tracking changes 
    /// to the workflow collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public interface IWorkflowManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when an workflow is added.
        /// </summary>
        event EventHandler<Workflow> WorkflowAdded;

        /// <summary>
        /// An event that fires when an workflow is udpated.
        /// </summary>
        event EventHandler<Workflow> WorkflowUpdated;

        /// <summary>
        /// An event that fires when an workflow is removed.
        /// </summary>
        event EventHandler<Workflow> WorkflowRemoved;

        /// <summary>
        /// Returns a workflow based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow.</param>
        /// <returns>The workflow.</returns>
        Workflow GetWorkflow(Guid workflowId);

        /// <summary>
        /// Returns a workflow based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow.</param>
        /// <returns>The workflow.</returns>
        Workflow GetWorkflow(WorkflowIdParameter workflowId);

        /// <summary>
        /// Retrieves a collection of workflows that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>
        /// An enumerable collection of workflows that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Workflow> GetWorkflows(ClassIdParameter classId);

        /// <summary>
        /// Retrieves a collection of workflows that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned workflows. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of workflows that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Workflow> GetWorkflows(IQuery<Workflow> query);

        /// <summary>
        /// Retrieves a collection of workflows that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned workflows. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of workflows that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        IEnumerable<Workflow> GetWorkflows(IQuery<Workflow> query, IQueryContext context);

        /// <summary>
        /// Adds a workflow to the manager.
        /// </summary>
        /// <param name="workflowEntity">The workflow to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IWorkflowManager Add(Workflow workflowEntity);

        /// <summary>
        /// Update a workflow to the manager.
        /// </summary>
        /// <param name="workflowEntity">The workflow to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IWorkflowManager Update(Workflow workflowEntity);

        /// <summary>
        /// Removes the specified workflow from the manager.
        /// </summary>
        /// <remarks>This method removes the specified workflow from the manager. If the workflow does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="workflowId">The workflow id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IWorkflowManager Remove(Guid workflowId);
    }
}
