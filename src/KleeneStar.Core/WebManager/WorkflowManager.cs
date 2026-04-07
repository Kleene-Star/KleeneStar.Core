using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
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
    /// Defines the contract for managing workflows, including adding, retrieving, and removing, as well as
    /// handling workflow-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing workflows and events for tracking changes 
    /// to the workflow collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public sealed class WorkflowManager : IWorkflowManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when an workflow is added.
        /// </summary>
        public event EventHandler<Workflow> WorkflowAdded;

        /// <summary>
        /// An event that fires when an workflow is udpated.
        /// </summary>
        public event EventHandler<Workflow> WorkflowUpdated;

        /// <summary>
        /// An event that fires when an workflow is removed.
        /// </summary>
        public event EventHandler<Workflow> WorkflowRemoved;

        /// <summary>
        /// Returns the collection of workspace keys that are reserved and cannot be used for custom workspaces.
        /// </summary>
        /// <remarks>
        /// The reserved keys typically represent system-defined workspaces and are not available
        /// for user-defined or custom workspace creation.
        /// </remarks>
        public static IEnumerable<string> ReservedWorkflowNames =>
        [
            "default", "admin", "system", "assets", "api", "workspace",
            "workspaces", "icons", "setting"
        ];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private WorkflowManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns a workflow based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow.</param>
        /// <returns>The workflow.</returns>
        public Workflow GetWorkflow(Guid workflowId)
        {
            var query = new Query<Workflow>()
                .Where(x => x.Id == workflowId)
                .WithPaging(0, 1);

            return ModelHub.GetWorkflows(query)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns a workflow based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow.</param>
        /// <returns>The workflow.</returns>
        public Workflow GetWorkflow(WorkflowIdParameter workflowId)
        {
            var guid = Guid.TryParse(workflowId.Value, out Guid id) ? id : Guid.Empty;

            return GetWorkflow(guid);
        }

        /// <summary>
        /// Retrieves a collection of workflows that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>
        /// An enumerable collection of workflows that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Workflow> GetWorkflows(ClassIdParameter classId)
        {
            var guid = Guid.TryParse(classId.Value, out Guid id) ? id : Guid.Empty;
            var query = new Query<Workflow>()
                .WhereEquals(x => x.ClassId, guid)
                .WithPaging(0, 1);

            return ModelHub.GetWorkflows(query);
        }

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
        public IEnumerable<Workflow> GetWorkflows(IQuery<Workflow> query)
        {
            return ModelHub.GetWorkflows(query);
        }

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
        public IEnumerable<Workflow> GetWorkflows(IQuery<Workflow> query, IQueryContext context)
        {
            return ModelHub.GetWorkflows(query, context as KleeneStarDbContext);
        }

        /// <summary>
        /// Adds a workflow to the manager.
        /// </summary>
        /// <param name="workflowEntity">The workflow to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkflowManager Add(Workflow workflowEntity)
        {
            ArgumentNullException.ThrowIfNull(workflowEntity);

            ModelHub.Add(workflowEntity);

            WorkflowAdded?.Invoke(this, workflowEntity);

            // create notification
            CoreHub.AddNotification("Create", "success", 5000);

            return this;
        }

        /// <summary>
        /// Update a workflow to the manager.
        /// </summary>
        /// <param name="workflowEntity">The workflow to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkflowManager Update(Workflow workflowEntity)
        {
            ArgumentNullException.ThrowIfNull(workflowEntity);

            ModelHub.Update(workflowEntity);

            WorkflowUpdated?.Invoke(this, workflowEntity);

            // create notification
            CoreHub.AddNotification("Clone", "success", 5000);

            return this;
        }

        /// <summary>
        /// Removes the specified workflow from the manager.
        /// </summary>
        /// <remarks>This method removes the specified workflow from the manager. If the workflow does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="workflowId">The workflow id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkflowManager Remove(Guid workflowId)
        {
            var workflowEntry = GetWorkflow(workflowId);

            if (workflowEntry is not null)
            {
                ModelHub.Remove(workflowEntry);
                WorkflowRemoved?.Invoke(this, workflowEntry);
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
