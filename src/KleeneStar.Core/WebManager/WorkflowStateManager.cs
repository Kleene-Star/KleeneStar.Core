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
    /// Defines the contract for managing workflow states, including adding, retrieving, and removing, as well as
    /// handling workflow states-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing workflow statess and events for tracking changes 
    /// to the workflow state collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public sealed class WorkflowStateManager : IWorkflowStateManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when an workflow state is added.
        /// </summary>
        public event EventHandler<WorkflowState> StateAdded;

        /// <summary>
        /// An event that fires when an workflow state is udpated.
        /// </summary>
        public event EventHandler<WorkflowState> StateUpdated;

        /// <summary>
        /// An event that fires when an workflow state is removed.
        /// </summary>
        public event EventHandler<WorkflowState> StateRemoved;

        /// <summary>
        /// Returns the collection of workspace state names that are reserved and cannot be used for custom workspaces.
        /// </summary>
        /// <remarks>
        /// The reserved keys typically represent system-defined workspaces and are not available
        /// for user-defined or custom workspace creation.
        /// </remarks>
        public static IEnumerable<string> ReservedStateNames =>
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
        private WorkflowStateManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Returns a workflow state based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow state.</param>
        /// <returns>The workflow state.</returns>
        public WorkflowState GetState(Guid workflowId)
        {
            var query = new Query<WorkflowState>()
                .Where(x => x.Id == workflowId)
                .WithPaging(0, 1);

            return ModelHub.GetWorkflowStates(query)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns a workflow state based on its id.
        /// </summary>
        /// <param name="workflowId">The id of the workflow state.</param>
        /// <returns>The workflow state.</returns>
        public WorkflowState GetState(WorkflowStateIdParameter workflowId)
        {
            var guid = Guid.TryParse(workflowId.Value, out Guid id) ? id : Guid.Empty;

            return GetState(guid);
        }

        /// <summary>
        /// Retrieves a collection of workflow states that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="classId">The id of the class.</param>
        /// <returns>
        /// An enumerable collection of workflow states that match the given predicate. If no class 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<WorkflowState> GetStates(ClassIdParameter classId)
        {
            var guid = Guid.TryParse(classId.Value, out Guid id) ? id : Guid.Empty;
            var query = new Query<WorkflowState>()
                .WhereEquals(x => x.ClassId, guid)
                .WithPaging(0, 1);

            return ModelHub.GetWorkflowStates(query);
        }

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
        public IEnumerable<WorkflowState> GetStates(IQuery<WorkflowState> query)
        {
            return ModelHub.GetWorkflowStates(query);
        }

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
        public IEnumerable<WorkflowState> GetStates(IQuery<WorkflowState> query, IQueryContext context)
        {
            return ModelHub.GetWorkflowStates(query, context as KleeneStarDbContext);
        }

        /// <summary>
        /// Adds a workflow state to the manager.
        /// </summary>
        /// <param name="stateEntity">The workflow state to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkflowStateManager Add(WorkflowState stateEntity)
        {
            ArgumentNullException.ThrowIfNull(stateEntity);

            ModelHub.Add(stateEntity);

            StateAdded?.Invoke(this, stateEntity);

            // create notification
            CoreHub.AddNotification("Create", "success", 5000);

            return this;
        }

        /// <summary>
        /// Update a workflow state to the manager.
        /// </summary>
        /// <param name="stateEntity">The workflow state to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkflowStateManager Update(WorkflowState stateEntity)
        {
            ArgumentNullException.ThrowIfNull(stateEntity);

            ModelHub.Update(stateEntity);

            StateUpdated?.Invoke(this, stateEntity);

            // create notification
            CoreHub.AddNotification("Clone", "success", 5000);

            return this;
        }

        /// <summary>
        /// Removes the specified workflow state from the manager.
        /// </summary>
        /// <remarks>This method removes the specified workflow state from the manager. If the workflow state does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="stateId">The workflow state id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkflowStateManager Remove(Guid stateId)
        {
            var steteEntry = GetState(stateId);

            if (steteEntry is not null)
            {
                ModelHub.Remove(steteEntry);
                StateRemoved?.Invoke(this, steteEntry);
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
