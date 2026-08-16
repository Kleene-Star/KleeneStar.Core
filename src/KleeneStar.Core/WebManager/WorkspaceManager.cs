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
        /// Gets the collection of workspace keys that are reserved and cannot be used for custom workspaces.
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
        /// Returns a workspace based on its id.
        /// </summary>
        /// <param name="workspaceId">The id of the workspace.</param>
        /// <returns>The workspace.</returns>
        public Workspace GetWorkspace(Guid workspaceId)
        {
            var query = new Query<Workspace>()
                .Where(x => x.Id == workspaceId)
                .WithPaging(0, 1);

            return ModelHub.GetWorkspaces(query)
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

            var query = new Query<Workspace>()
                .WhereEqualsIgnoreCase(x => x.Key, key)
                .WithPaging(0, 1);

            return ModelHub.GetWorkspaces(query)
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a collection of categories that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// The query used to filter and select categories. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of categories that satisfy the query conditions. The 
        /// collection is empty if no categories match.
        /// </returns>
        public IEnumerable<Category> GetCategories(IQuery<Category> query)
        {
            return ModelHub.GetCategories(query);
        }

        /// <summary>
        /// Retrieves a collection of workspaces that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned workspaces. Must not be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of workspaces that match the given predicate. If no workspaces 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Workspace> GetWorkspaces(IQuery<Workspace> query)
        {
            return ModelHub.GetWorkspaces(query);
        }

        /// <summary>
        /// Retrieves a collection of workspaces that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned workspaces. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of workspaces that match the given predicate. If no workspaces 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<Workspace> GetWorkspaces(IQuery<Workspace> query, IQueryContext context)
        {
            return ModelHub.GetWorkspaces(query, context as KleeneStarDbContext);
        }

        /// <summary>
        /// Returns the active workspaces the supplied identity has favorited, ordered by name.
        /// Backs the pinned section at the top of the workspace dropdown.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <returns>The favorited workspaces. The collection may be empty.</returns>
        public IReadOnlyList<Workspace> GetFavoriteWorkspaces(Guid ownerId)
        {
            return [.. ModelHub.GetWorkspaceBookmarks(new Query<WorkspaceBookmark>())
                .Where(x => x.OwnerId == ownerId
                    && x.Favorite
                    && x.Workspace is not null
                    && x.Workspace.State == WorkspaceState.Active)
                .OrderBy(x => x.Workspace.Name, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Workspace)];
        }

        /// <summary>
        /// Returns the active workspaces the supplied identity has most recently visited,
        /// newest first, capped at <paramref name="count"/>. Backs the "recently used"
        /// section of the workspace dropdown.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="count">The maximum number of workspaces to return.</param>
        /// <returns>The recently visited workspaces, newest first. The collection may be empty.</returns>
        public IReadOnlyList<Workspace> GetRecentWorkspaces(Guid ownerId, int count)
        {
            return [.. ModelHub.GetWorkspaceBookmarks(new Query<WorkspaceBookmark>())
                .Where(x => x.OwnerId == ownerId
                    && x.LastVisited != default
                    && x.Workspace is not null
                    && x.Workspace.State == WorkspaceState.Active)
                .OrderByDescending(x => x.LastVisited)
                .Take(Math.Max(0, count))
                .Select(x => x.Workspace)];
        }

        /// <summary>
        /// Returns whether the supplied identity has favorited the supplied workspace.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="workspaceId">The id of the workspace.</param>
        /// <returns><see langword="true"/> when the workspace is favorited by the identity.</returns>
        public bool IsFavorite(Guid ownerId, Guid workspaceId)
        {
            return ModelHub.GetWorkspaceBookmark(ownerId, workspaceId)?.Favorite ?? false;
        }

        /// <summary>
        /// Sets the favorite state of the supplied workspace for the supplied identity,
        /// inserting or updating the backing bookmark. Returns <see langword="null"/> when the
        /// owner or workspace does not exist.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="workspaceId">The id of the workspace.</param>
        /// <param name="favorite">The new favorite state.</param>
        /// <returns>The persisted bookmark, or <see langword="null"/>.</returns>
        public WorkspaceBookmark SetFavorite(Guid ownerId, Guid workspaceId, bool favorite)
        {
            var bookmark = ModelHub.UpsertWorkspaceBookmark(ownerId, workspaceId, favorite, recordVisit: false);

            if (bookmark is not null)
            {
                WorkspaceUpdated?.Invoke(this, GetWorkspace(workspaceId));

                // confirmation toast (pushed over the message queue; harmless when the host is not wired)
                CoreHub.AddNotification
                (
                    favorite ? "kleenestar.core:notification.title.favorited" : "kleenestar.core:notification.title.unfavorited",
                    favorite ? "kleenestar.core:notification.workspace.favorited" : "kleenestar.core:notification.workspace.unfavorited",
                    5000
                );
            }

            return bookmark;
        }

        /// <summary>
        /// Records that the supplied identity has just opened the supplied workspace by
        /// advancing the bookmark's last-visited timestamp (inserting the bookmark when needed).
        /// The mutation is deliberately quiet because it fires on every workspace page load.
        /// Returns <see langword="null"/> when the owner or workspace does not exist.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="workspaceId">The id of the workspace.</param>
        /// <returns>The persisted bookmark, or <see langword="null"/>.</returns>
        public WorkspaceBookmark RecordVisit(Guid ownerId, Guid workspaceId)
        {
            return ModelHub.UpsertWorkspaceBookmark(ownerId, workspaceId, favorite: null, recordVisit: true);
        }

        /// <summary>
        /// Adds a workspace to the workspace manager.
        /// </summary>
        /// <param name="workspace">The workspace to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkspaceManager Add(Workspace workspace)
        {
            ArgumentNullException.ThrowIfNull(workspace);

            ModelHub.Add(workspace);

            WorkspaceAdded?.Invoke(this, workspace);

            // create notification
            CoreHub.AddNotification("kleenestar.core:notification.title.created", "kleenestar.core:notification.workspace.created", workspace);

            return this;
        }

        /// <summary>
        /// Update a workspace to the workspace manager.
        /// </summary>
        /// <param name="workspace">The workspace to updated. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkspaceManager Update(Workspace workspace)
        {
            ArgumentNullException.ThrowIfNull(workspace);

            ModelHub.Update(workspace);

            WorkspaceUpdated?.Invoke(this, workspace);

            // create notification
            CoreHub.AddNotification("kleenestar.core:notification.title.updated", "kleenestar.core:notification.workspace.updated", workspace);

            return this;
        }

        /// <summary>
        /// Removes the specified workspace from the workspace manager.
        /// </summary>
        /// <remarks>This method removes the specified workspace from the manager. If the workspace does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="workspaceId">The workspace id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkspaceManager Remove(Guid workspaceId)
        {
            var workspace = GetWorkspace(workspaceId);

            if (workspace is not null)
            {
                ModelHub.Remove(workspace);
                WorkspaceRemoved?.Invoke(this, workspace);

                // remove notification (only when a workspace was actually removed)
                CoreHub.AddNotification("kleenestar.core:notification.title.deleted", "kleenestar.core:notification.workspace.deleted", workspace);
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
