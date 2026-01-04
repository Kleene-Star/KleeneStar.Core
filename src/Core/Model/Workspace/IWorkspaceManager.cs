using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Core.Model.Workspace
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
    public interface IWorkspaceManager : IComponentManager
    {
        /// <summary>
        /// An event that fires when an workspace is added.
        /// </summary>
        event EventHandler<IWorkspace> WorkspaceAdded;

        /// <summary>
        /// An event that fires when an workspace is udpated.
        /// </summary>
        event EventHandler<IWorkspace> WorkspaceUpdated;

        /// <summary>
        /// An event that fires when an workspace is removed.
        /// </summary>
        event EventHandler<IWorkspace> WorkspaceRemoved;

        /// <summary>
        /// Returns all workspaces.
        /// </summary>
        IEnumerable<IWorkspace> Workspaces { get; }

        /// <summary>
        /// Returns the collection of category names associated with the workspace.
        /// </summary>
        IEnumerable<string> WorkspaceCategories { get; }

        /// <summary>
        /// Adds a workspace to the workspace manager.
        /// </summary>
        /// <param name="workspace">The workspace to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IWorkspaceManager AddWorkspace(IWorkspace workspace);

        /// <summary>
        /// Returns a workspace based on its id.
        /// </summary>
        /// <param name="workspaceId">The id of the workspace.</param>
        /// <returns>The workspace.</returns>
        IWorkspace GetWorkspace(Guid workspaceId);

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
        IWorkspace GetWorkspaceByKey(string key);

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
        IEnumerable<IWorkspace> GetWorkspaces(Expression<Func<IWorkspace, bool>> predicate);

        /// <summary>
        /// Removes the specified workspace from the workspace manager.
        /// </summary>
        /// <remarks>This method removes the specified workspace from the manager. If the workspace does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="workspaceId">The workspace id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        IWorkspaceManager RemoveWorkspace(Guid workspaceId);
    }
}
