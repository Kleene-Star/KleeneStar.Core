using KleeneStar.Model;
using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Workflows
{
    /// <summary>
    /// Provides functionality for editing workflows through a REST API interface.
    /// Supports retrieval of workflow structure including states and transitions
    /// for the visual workflow editor control.
    /// </summary>
    [Title("Workflow editor")]
    public sealed class WorkflowEditor : RestApiWorkflow
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WorkflowEditor()
        {
        }

        /// <summary>
        /// Creates a query context backed by the application's database.
        /// </summary>
        /// <returns>The shared <see cref="KleeneStarDbContext"/>.</returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves the workflow identified by the specified identifier, including its
        /// associated states and transitions.
        /// </summary>
        /// <param name="workflowId">
        /// The unique identifier of the workflow to retrieve.
        /// </param>
        /// <param name="context">
        /// The query context providing access to the underlying data store. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The current API request. Cannot be null.
        /// </param>
        /// <returns>
        /// A <see cref="RestApiWorkflowResult"/> representing the workflow, or <c>null</c>
        /// when no matching workflow exists.
        /// </returns>
        protected override RestApiWorkflowResult Retrieve(string workflowId, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(workflowId, out var guid))
            {
                return null;
            }

            var db = context as KleeneStarDbContext;
            var workflow = db.Workflows
                .Include(w => w.Statuses)
                    .ThenInclude(s => s.Category)
                .Include(w => w.Transitions)
                .AsNoTracking()
                .FirstOrDefault(w => w.Id == guid);

            if (workflow is null)
            {
                return null;
            }

            return new RestApiWorkflowResult()
            {
                Id = workflow.Id.ToString(),
                Name = workflow.Name,
                Description = workflow.Description,
                State = workflow.State.ToString(),
                Version = "1",
                States = workflow.Statuses?
                    .Where(s => s.State == StatusState.Active)
                    .Select(s => new RestApiWorkflowState()
                    {
                        Id = s.Id.ToString(),
                        Label = s.Name,
                        BackgroundColor = s.Category?.Color ?? "#6c757d",
                        ForegroundColor = "#ffffff",
                        Icon = s.Icon?.ToString()
                    }),
                Transitions = workflow.Transitions?
                    .Where(t => t.State == TransitionState.Active)
                    .Select(t => new RestApiWorkflowTransition()
                    {
                        Id = t.Id.ToString(),
                        From = t.SourceId.ToString(),
                        To = t.TargetId.ToString(),
                        Label = t.Name,
                        Description = t.Description
                    })
            };
        }

        /// <summary>
        /// Retrieves the available states for the workflow identified by the specified identifier.
        /// </summary>
        /// <param name="workflowId">
        /// The unique identifier of the workflow whose states are to be retrieved.
        /// </param>
        /// <param name="context">
        /// The query context providing access to the underlying data store. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The current API request. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of <see cref="RestApiWorkflowState"/> representing the
        /// active states of the workflow. The collection may be empty if no states are defined.
        /// </returns>
        protected override IEnumerable<RestApiWorkflowState> RetrieveStates(string workflowId, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(workflowId, out var guid))
            {
                return [];
            }

            var db = context as KleeneStarDbContext;
            var workflow = db.Workflows
                .Include(w => w.Statuses)
                    .ThenInclude(s => s.Category)
                .AsNoTracking()
                .FirstOrDefault(w => w.Id == guid);

            if (workflow?.Statuses is null)
            {
                return [];
            }

            return workflow.Statuses
                .Where(s => s.State == StatusState.Active)
                .Select(s => new RestApiWorkflowState()
                {
                    Id = s.Id.ToString(),
                    Label = s.Name,
                    BackgroundColor = s.Category?.Color ?? "#6c757d",
                    ForegroundColor = "#ffffff",
                    Icon = s.Icon?.ToString()
                });
        }

        /// <summary>
        /// Retrieves the transitions for the workflow identified by the specified identifier.
        /// </summary>
        /// <param name="workflowId">
        /// The unique identifier of the workflow whose transitions are to be retrieved.
        /// </param>
        /// <param name="context">
        /// The query context providing access to the underlying data store. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The current API request. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of <see cref="RestApiWorkflowTransition"/> representing the
        /// active transitions of the workflow. The collection may be empty if no transitions are defined.
        /// </returns>
        protected override IEnumerable<RestApiWorkflowTransition> RetrieveTransitions(string workflowId, IQueryContext context, IRequest request)
        {
            if (!Guid.TryParse(workflowId, out var guid))
            {
                return [];
            }

            var db = context as KleeneStarDbContext;
            var transitions = db.Transitions
                .Where(t => t.WorkflowId == guid && t.State == TransitionState.Active)
                .AsNoTracking()
                .ToList();

            return transitions.Select(t => new RestApiWorkflowTransition()
            {
                Id = t.Id.ToString(),
                From = t.SourceId.ToString(),
                To = t.TargetId.ToString(),
                Label = t.Name,
                Description = t.Description
            });
        }

        /// <summary>
        /// Retrieves the collection of validators to be applied for the specified workflow operation.
        /// </summary>
        /// <param name="workflowId">
        /// The unique identifier of the workflow operation for which validations are requested.
        /// </param>
        /// <param name="context">
        /// The query context that provides access to data and services relevant to the validation process.
        /// </param>
        /// <param name="request">
        /// The request object containing details about the current API request.
        /// </param>
        /// <returns>
        /// An enumerable collection of <see cref="RestApiWorkflowValidator"/> instances representing the 
        /// validations to apply. The collection may be empty if no validations are required.
        /// </returns>
        protected override IEnumerable<RestApiWorkflowValidator> RetrieveValidations(string workflowId, IQueryContext context, IRequest request)
        {
            // return empty by default
            return [];
        }

        /// <summary>
        /// Retrieves the collection of post functions associated with the specified identifier.
        /// </summary>
        /// <param name="workflowId">
        /// The unique identifier for which to retrieve post functions.
        /// </param>
        /// <param name="context">
        /// The query context that provides access to data and services required for retrieval.
        /// </param>
        /// <param name="request">
        /// The request information relevant to the retrieval operation.
        /// </param>
        /// <returns>
        /// An enumerable collection of post functions associated with the specified identifier. Returns 
        /// an empty collection if no post functions are found.
        /// </returns>
        protected override IEnumerable<RestApiWorkflowPostFunction> RetrievePostFunctions(string workflowId, IQueryContext context, IRequest request)
        {
            // return empty by default
            return [];
        }

        /// <summary>
        /// Retrieves the collection of workflow guards associated with the specified workflow.
        /// </summary>
        /// <param name="workflowId">
        /// The unique identifier of the workflow for which to retrieve guards.
        /// </param>
        /// <param name="context">
        /// The query context that provides access to relevant data and services during guard retrieval.
        /// </param>
        /// <param name="request">
        /// The request object containing details about the current API operation.
        /// </param>
        /// <returns>
        /// An enumerable collection of <see cref="RestApiWorkflowGuard"/> instances representing the guards 
        /// for the specified workflow. Returns an empty collection if no guards are defined.
        /// </returns>
        protected override IEnumerable<RestApiWorkflowGuard> RetrieveGuards(string workflowId, IQueryContext context, IRequest request)
        {
            // return empty by default
            return [];
        }
    }
}
