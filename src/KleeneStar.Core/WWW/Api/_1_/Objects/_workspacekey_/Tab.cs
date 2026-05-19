using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_
{
    /// <summary>
    /// REST API endpoint that returns and mutates the tab views (<see cref="ObjectView"/>)
    /// configured for a workspace's objects index.
    /// </summary>
    /// <remarks>
    /// Each <see cref="RestApiTabView"/> returned by <see cref="RetrieveViews"/> binds an
    /// <see cref="ObjectView"/> to one of the tab template fragments registered on the
    /// objects tab (Table, List, Dashboard, Kanban, Scrum Sprint, Scrum Backlog).
    /// </remarks>
    [Title("kleenestar.core:object.tab.header")]
    [Cache]
    public sealed class Tab : RestApiTab<Model.Entities.Object>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Tab()
        {
        }

        /// <summary>
        /// Creates a new instance of an object that implements the IQueryContext interface.
        /// </summary>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Returns the persisted object views for the workspace identified by the
        /// route's workspace key, in display order.
        /// </summary>
        protected override IEnumerable<RestApiTabView> RetrieveViews(IQueryContext context, IRequest request)
        {
            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceKey);

            if (workspace is null)
            {
                yield break;
            }

            var views = CoreHub.ObjectViewManager
                .GetViewsForWorkspace(workspace.Id)
                .Where(x => x.State == ObjectViewState.Active);

            foreach (var view in views)
            {
                yield return new RestApiTabView
                {
                    Id = view.Id.ToString(),
                    Name = view.Name,
                    Title = view.Name,
                    Icon = (view.ViewType.Icon() as WebExpress.WebUI.WebIcon.Icon)?.Class,
                    TemplateId = view.ViewType.TemplateId(),
                    Uri = ResolveContentUri(view.ViewType, request)?.ToString()
                };
            }
        }

        /// <summary>
        /// Persists a new <see cref="ObjectView"/> for the workspace, defaulting to the
        /// view type whose <see cref="ObjectViewTypeExtensions.TemplateId"/> matches the
        /// supplied <paramref name="templateId"/>.
        /// </summary>
        protected override IRestApiTabView CreateView(IQueryContext context, IRequest request, string templateId)
        {
            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceKey);

            if (workspace is null)
            {
                return null;
            }

            var viewType = ResolveViewType(templateId);
            var existing = CoreHub.ObjectViewManager.GetViewsForWorkspace(workspace.Id).ToList();
            var name = BuildUniqueName(existing, viewType.ToString());

            var view = new ObjectView
            {
                Id = Guid.NewGuid(),
                Name = name,
                ViewType = viewType,
                Order = existing.Count == 0 ? 0 : existing.Max(x => x.Order) + 1,
                State = ObjectViewState.Active,
                WorkspaceId = workspace.Id,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            CoreHub.ObjectViewManager.AddObjectView(view);

            return new RestApiTabView
            {
                Id = view.Id.ToString(),
                Name = view.Name,
                Title = view.Name,
                Icon = (viewType.Icon() as WebExpress.WebUI.WebIcon.Icon)?.Class,
                TemplateId = viewType.TemplateId(),
                Uri = ResolveContentUri(viewType, request)?.ToString()
            };
        }

        /// <summary>
        /// Removes the <see cref="ObjectView"/> identified by <paramref name="viewId"/>.
        /// </summary>
        protected override bool RemoveView(string viewId)
        {
            if (!Guid.TryParse(viewId, out var guid))
            {
                return false;
            }

            var view = CoreHub.ObjectViewManager.GetObjectView(guid);

            if (view is null)
            {
                return false;
            }

            CoreHub.ObjectViewManager.RemoveObjectView(view);

            return true;
        }

        /// <summary>
        /// Maps a tab-template id back to the corresponding <see cref="ObjectViewType"/>.
        /// Falls back to <see cref="ObjectViewType.Table"/> when the id is unknown.
        /// </summary>
        private static ObjectViewType ResolveViewType(string templateId)
        {
            if (string.IsNullOrWhiteSpace(templateId))
            {
                return ObjectViewType.Table;
            }

            foreach (ObjectViewType candidate in Enum.GetValues<ObjectViewType>())
            {
                if (string.Equals(candidate.TemplateId(), templateId, StringComparison.OrdinalIgnoreCase))
                {
                    return candidate;
                }
            }

            return ObjectViewType.Table;
        }

        /// <summary>
        /// Resolves the content REST endpoint URI for the given view type and binds the
        /// current request's workspace key. The returned URI is sent in
        /// <see cref="RestApiTabView.Uri"/> and the client-side <c>BindTemplate</c> on each
        /// tab-template fragment routes it into the <c>data-uri</c> of the active tab's
        /// inner content control.
        /// </summary>
        private static IUri ResolveContentUri(ObjectViewType type, IRequest request)
        {
            var uri = type switch
            {
                ObjectViewType.Table => CoreHub.GetUri<Table>(),
                ObjectViewType.List => CoreHub.GetUri<List>(),
                ObjectViewType.Dashboard => CoreHub.GetUri<Dashboard>(),
                ObjectViewType.Kanban => CoreHub.GetUri<Kanban>(),
                ObjectViewType.ScrumSprint => CoreHub.GetUri<ScrumSprint>(),
                ObjectViewType.ScrumBacklog => CoreHub.GetUri<ScrumBacklog>(),
                _ => null
            };

            return uri?.BindParameters(request);
        }

        /// <summary>
        /// Returns a name based on <paramref name="seed"/> that doesn't collide with the
        /// existing views in the workspace.
        /// </summary>
        private static string BuildUniqueName(IEnumerable<ObjectView> existing, string seed)
        {
            var taken = existing
                .Select(x => x.Name ?? string.Empty)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!taken.Contains(seed))
            {
                return seed;
            }

            for (var i = 2; i < 1000; i++)
            {
                var candidate = $"{seed} ({i})";
                if (!taken.Contains(candidate))
                {
                    return candidate;
                }
            }

            return $"{seed} ({Guid.NewGuid():N})";
        }
    }
}
