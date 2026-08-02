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
        /// An event that fires after a state change has been applied to an object.
        /// </summary>
        public event EventHandler<WorkflowTransitionResult> TransitionExecuted;

        /// <summary>
        /// Gets the collection of workflow names that are reserved and cannot be used for custom workflows.
        /// </summary>
        /// <remarks>
        /// The reserved names typically represent system-defined routes and are not available
        /// for user-defined or custom workflow creation.
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
        /// Returns a workflow together with the structure its state machine is made of: its
        /// participating states (including their category) and its transitions with both
        /// endpoints resolved.
        /// </summary>
        /// <param name="workflowId">The id of the workflow.</param>
        /// <returns>The workflow including states and transitions, or <c>null</c>.</returns>
        public Workflow GetWorkflowWithStructure(Guid workflowId)
        {
            return ModelHub.GetWorkflowWithStructure(workflowId);
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
                .WhereEquals(x => x.ClassId, guid);

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
        /// Resolves the persisted payload of a workflow-backed field to one of the workflow's
        /// states, first by normalized name and then by status id.
        /// </summary>
        /// <param name="workflow">The workflow whose states are searched.</param>
        /// <param name="data">The persisted payload of the workflow field.</param>
        /// <returns>The matching state, or <c>null</c>.</returns>
        public Status ResolveStatus(Workflow workflow, string data)
        {
            if (workflow?.Statuses is null || string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            var normalized = Normalize(data);

            return workflow.Statuses.FirstOrDefault(x => Normalize(x.Name) == normalized)
                ?? workflow.Statuses.FirstOrDefault(x => string.Equals(x.Id.ToString(), data, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the states an object in the supplied state may be moved to: the targets of the
        /// active transitions leaving it, or the workflow's entry states when the object has not
        /// entered the state machine yet.
        /// </summary>
        /// <param name="workflow">The workflow to walk.</param>
        /// <param name="currentStatus">The state to leave, or <c>null</c>.</param>
        /// <returns>The reachable states, without duplicates.</returns>
        public IEnumerable<Status> GetTargetStatuses(Workflow workflow, Status currentStatus)
        {
            if (workflow is null)
            {
                return [];
            }

            if (currentStatus is null)
            {
                return GetEntryStatuses(workflow);
            }

            var participating = (workflow.Statuses ?? [])
                .Where(x => x.State == StatusState.Active)
                .ToDictionary(x => x.Id);

            return [.. (workflow.Transitions ?? [])
                .Where(x => x.State == TransitionState.Active)
                .Where(x => x.SourceId == currentStatus.Id)
                .Where(x => x.TargetId != currentStatus.Id)
                .Select(x => x.Target ?? (participating.TryGetValue(x.TargetId, out var target) ? target : null))
                .Where(x => x is not null && x.State == StatusState.Active)
                .GroupBy(x => x.Id)
                .Select(x => x.First())];
        }

        /// <summary>
        /// Moves a workflow-backed field of an object to the requested state, enforcing the
        /// workflow server-side.
        /// </summary>
        /// <remarks>
        /// The stages run in the order the workflow concept prescribes: guard, validator, apply,
        /// post function. Transition-level guards, validators and post functions are not part of
        /// the data model yet — <see cref="Transition"/> carries no rule collections — so those
        /// stages resolve an empty configuration and pass. The reachability check and the
        /// built-in post function (stamping the updater and raising
        /// <see cref="TransitionExecuted"/>) always run.
        /// </remarks>
        /// <param name="objectId">The id of the object whose state changes.</param>
        /// <param name="fieldId">The id of the workflow-backed field carrying the state.</param>
        /// <param name="targetStatusId">The id of the requested state.</param>
        /// <param name="identityId">The identity performing the change.</param>
        /// <returns>The outcome of the state change.</returns>
        public WorkflowTransitionResult ExecuteTransition(Guid objectId, Guid fieldId, Guid targetStatusId, Guid identityId)
        {
            var objectEntity = CoreHub.ObjectManager.GetObject(objectId);
            var field = CoreHub.FieldManager.GetField(fieldId);

            if (objectEntity is null ||
                field is null ||
                field.ClassId != objectEntity.ClassId ||
                field.FieldType != FieldType.Workflow ||
                !field.WorkflowId.HasValue)
            {
                return Failed(WorkflowTransitionOutcome.NotFound, "kleenestar.core:object.property.workflow.transition.notfound", objectId, fieldId);
            }

            var workflow = GetWorkflowWithStructure(field.WorkflowId.Value);
            var target = (workflow?.Statuses ?? []).FirstOrDefault(x => x.Id == targetStatusId);

            if (workflow is null || target is null)
            {
                return Failed(WorkflowTransitionOutcome.NotFound, "kleenestar.core:object.property.workflow.transition.notfound", objectId, fieldId);
            }

            var value = CoreHub.ValueManager.GetValue(objectId, fieldId);
            var source = ResolveStatus(workflow, value?.Data);

            if (source is not null && source.Id == target.Id)
            {
                return Failed(WorkflowTransitionOutcome.Unchanged, "kleenestar.core:object.property.workflow.transition.unchanged", objectId, fieldId, source, target);
            }

            // guard stage - an archived definition is read-only and no longer governs objects, and
            // the move has to follow an active transition; an object that has not entered the
            // state machine yet may only be placed on an entry state
            if (workflow.State != WorkflowState.Active)
            {
                return Failed(WorkflowTransitionOutcome.NotAllowed, "kleenestar.core:object.property.workflow.transition.notallowed", objectId, fieldId, source, target);
            }

            var transition = source is null
                ? null
                : (workflow.Transitions ?? [])
                    .FirstOrDefault(x => x.State == TransitionState.Active && x.SourceId == source.Id && x.TargetId == target.Id);

            var allowed = source is null
                ? GetEntryStatuses(workflow).Any(x => x.Id == target.Id)
                : transition is not null;

            if (!allowed || target.State != StatusState.Active)
            {
                return Failed(WorkflowTransitionOutcome.NotAllowed, "kleenestar.core:object.property.workflow.transition.notallowed", objectId, fieldId, source, target);
            }

            // validator stage - the rules a transition validates against are configured on the
            // transition, which the data model does not carry yet, so there is nothing to check
            var validationErrors = Validate(transition);

            if (validationErrors.Count > 0)
            {
                return new WorkflowTransitionResult
                {
                    Outcome = WorkflowTransitionOutcome.ValidationFailed,
                    Message = "kleenestar.core:object.property.workflow.transition.invalid",
                    ObjectId = objectId,
                    FieldId = fieldId,
                    Source = source,
                    Target = target,
                    Transition = transition,
                    ValidationErrors = validationErrors
                };
            }

            // apply - the state travels as the status name, which is what the generic value
            // binder writes for a workflow field and what ResolveStatus reads back
            if (value is null)
            {
                CoreHub.ValueManager.Add(new Value
                {
                    ObjectId = objectId,
                    FieldId = fieldId,
                    Data = target.Name,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });
            }
            else
            {
                value.Data = target.Name;
                value.Updated = DateTime.UtcNow;

                CoreHub.ValueManager.Update(value);
            }

            var result = new WorkflowTransitionResult
            {
                Outcome = WorkflowTransitionOutcome.Executed,
                Message = "kleenestar.core:object.property.workflow.transition.executed",
                ObjectId = objectId,
                FieldId = fieldId,
                Source = source,
                Target = target,
                Transition = transition
            };

            RunPostFunctions(objectEntity, identityId, result);

            return result;
        }

        /// <summary>
        /// Runs the follow-up actions of a completed state change: the built-in ones that keep the
        /// object's audit trail honest and inform the user, followed by the post functions
        /// configured on the transition.
        /// </summary>
        /// <param name="objectEntity">The object that changed state.</param>
        /// <param name="identityId">The identity that performed the change.</param>
        /// <param name="result">The result of the state change.</param>
        private void RunPostFunctions(Model.Entities.Object objectEntity, Guid identityId, WorkflowTransitionResult result)
        {
            objectEntity.Updated = DateTime.UtcNow;

            // keep the previous updater when the caller is unauthenticated so the foreign key
            // never points at an empty identity
            if (identityId != Guid.Empty)
            {
                objectEntity.UpdaterId = identityId;
            }

            CoreHub.ObjectManager.Update(objectEntity);

            // the configured post functions of the transition would run here; the data model
            // carries none yet (see the remarks on ExecuteTransition)

            TransitionExecuted?.Invoke(this, result);
        }

        /// <summary>
        /// Runs the validators configured on a transition.
        /// </summary>
        /// <param name="transition">
        /// The transition being travelled, or <c>null</c> when the object enters the workflow at
        /// an entry state and therefore travels along none.
        /// </param>
        /// <returns>
        /// The findings, empty when the change may proceed. Always empty for now: validation rules
        /// live on the transition, which the data model does not carry yet.
        /// </returns>
        private static IReadOnlyList<string> Validate(Transition transition)
        {
            _ = transition;

            return [];
        }

        /// <summary>
        /// Returns the states the workflow may be entered at: the ones marked as entry states, or
        /// every participating state when the workflow declares none, so an object can still be
        /// brought into a workflow that has never been through the designer.
        /// </summary>
        /// <param name="workflow">The workflow to inspect.</param>
        /// <returns>The entry states.</returns>
        private static IEnumerable<Status> GetEntryStatuses(Workflow workflow)
        {
            var active = (workflow.Statuses ?? [])
                .Where(x => x.State == StatusState.Active)
                .ToList();

            var entryIds = (workflow.WorkflowStatuses ?? [])
                .Where(x => x.IsStart)
                .Select(x => x.StatusId)
                .ToHashSet();

            return entryIds.Count == 0
                ? active
                : [.. active.Where(x => entryIds.Contains(x.Id))];
        }

        /// <summary>
        /// Builds the result of a state change that did not take place.
        /// </summary>
        /// <param name="outcome">The verdict.</param>
        /// <param name="message">The internationalization key describing it.</param>
        /// <param name="objectId">The addressed object.</param>
        /// <param name="fieldId">The addressed field.</param>
        /// <param name="source">The state the object is in, when known.</param>
        /// <param name="target">The requested state, when known.</param>
        /// <returns>The result.</returns>
        private static WorkflowTransitionResult Failed
        (
            WorkflowTransitionOutcome outcome,
            string message,
            Guid objectId,
            Guid fieldId,
            Status source = null,
            Status target = null
        )
        {
            return new WorkflowTransitionResult
            {
                Outcome = outcome,
                Message = message,
                ObjectId = objectId,
                FieldId = fieldId,
                Source = source,
                Target = target
            };
        }

        /// <summary>
        /// Reduces a string to its lower-cased alphanumeric characters so loosely-formatted status
        /// slugs can be compared against status names.
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <returns>The normalized string.</returns>
        private static string Normalize(string value)
        {
            return new string((value ?? string.Empty).Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
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
            CoreHub.AddNotification("kleenestar.core:notification.title.created", "kleenestar.core:notification.workflow.created", 5000);

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

            // update notification
            CoreHub.AddNotification("kleenestar.core:notification.title.updated", "kleenestar.core:notification.workflow.updated", 5000);

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
