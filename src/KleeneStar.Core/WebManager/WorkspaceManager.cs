using KleeneStar.Model;
using KleeneStar.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Defines the contract for managing workspaces, including adding, retrieving, and removing workspaces, as well as
    /// handling workspace-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing workspaces and events for tracking changes 
    /// to the workspace collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public sealed class WorkspaceManager : IWorkspaceManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// An event that fires when an workspace is added.
        /// </summary>
        public event EventHandler<Workspace> WorkspaceAdded;

        /// <summary>
        /// An event that fires when an workspace is udpated.
        /// </summary>
        public event EventHandler<Workspace> WorkspaceUpdated;

        /// <summary>
        /// An event that fires when an workspace is removed.
        /// </summary>
        public event EventHandler<Workspace> WorkspaceRemoved;

        /// <summary>
        /// Returns all workspaces.
        /// </summary>
        public IEnumerable<Workspace> Workspaces => ModelHub.Workspaces;

        /// <summary>
        /// Returns the collection of workspace keys that are reserved and cannot be used for custom workspaces.
        /// </summary>
        /// <remarks>
        /// The reserved keys typically represent system-defined workspaces and are not available
        /// for user-defined or custom workspace creation.
        /// </remarks>
        public static IEnumerable<string> ReservedWorkspaceKeys =>
        [
            "default", "admin", "system", "assets", "api", "workspace",
            "workspaces", "icons", "setting"
        ];

        /// <summary>
        /// Returns the collection of category names associated with the workspace.
        /// </summary>
        public IEnumerable<string> Categories => ModelHub.Categories.Select(c => c.Name);

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private WorkspaceManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;
        }

        /// <summary>
        /// Adds a workspace to the workspace manager.
        /// </summary>
        /// <param name="workspace">The workspace to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkspaceManager AddWorkspace(Workspace workspace)
        {
            ArgumentNullException.ThrowIfNull(workspace);

            ModelHub.Add(workspace);

            WorkspaceAdded?.Invoke(this, workspace);

            return this;
        }

        /// <summary>
        /// Returns a workspace based on its id.
        /// </summary>
        /// <param name="workspaceId">The id of the workspace.</param>
        /// <returns>The workspace.</returns>
        public Workspace GetWorkspace(Guid workspaceId)
        {
            return ModelHub.GetWorkspaces(x => x.Id == workspaceId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the workspace associated with the specified unique key.
        /// </summary>
        /// <param name="key">
        /// The unique identifier for the workspace to retrieve. Cannot be null or empty.
        /// </param>
        /// <returns>
        /// An workspace corresponding to the specified key, or null if no matching 
        /// workspace is found.
        /// </returns>
        public Workspace GetWorkspaceByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            return Workspaces.Where(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a collection of workspaces that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="predicate"
        /// >An expression used to filter workspaces. Only workspaces for which the predicate 
        /// evaluates to true are included in the result.
        /// </param>
        /// <returns>
        /// An enumerable collection of workspaces that match the given predicate. If no workspaces 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Workspace> GetWorkspaces(Expression<Func<Workspace, bool>> predicate)
        {
            return ModelHub.GetWorkspaces(predicate);
        }

        /// <summary>
        /// Removes the specified workspace from the workspace manager.
        /// </summary>
        /// <remarks>This method removes the specified workspace from the manager. If the workspace does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="workspaceId">The workspace id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkspaceManager RemoveWorkspace(Guid workspaceId)
        {
            var workspace = GetWorkspace(workspaceId);

            if (workspace is not null)
            {
                ModelHub.Remove(workspace);
                WorkspaceRemoved?.Invoke(this, workspace);
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
