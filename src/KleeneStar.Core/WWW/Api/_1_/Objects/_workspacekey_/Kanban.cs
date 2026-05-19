using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_
{
    /// <summary>
    /// REST API kanban endpoint for the objects of a workspace. Columns are derived from
    /// the workspace's <see cref="WorkspaceState"/> values and swimlanes from the class
    /// each object belongs to.
    /// </summary>
    [Title("kleenestar.core:object.view.kanban.title")]
    [Cache]
    public sealed class Kanban : RestApiKanban<Model.Entities.Object>
    {
        /// <summary>
        /// Returns a <see cref="KleeneStarDbContext"/> so <see cref="CoreHub.ObjectManager"/>
        /// can run its queries; the base class' <see cref="WebExpress.WebIndex.Queries.DefaultQueryContext"/>
        /// would cast to null in the manager and trigger an NRE downstream.
        /// </summary>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Returns the kanban columns. Two columns are always rendered (Active, Archived).
        /// </summary>
        protected override IEnumerable<RestApiKanbanColumn> RetrieveColumns(IRequest request)
        {
            yield return new RestApiKanbanColumn
            {
                Id = WorkspaceState.Active.ToString(),
                Label = "Active",
                ColorCss = "wx-color-success"
            };

            yield return new RestApiKanbanColumn
            {
                Id = WorkspaceState.Archived.ToString(),
                Label = "Archived",
                ColorCss = "wx-color-secondary"
            };
        }

        /// <summary>
        /// Returns one swimlane per class that has at least one object in the workspace.
        /// </summary>
        protected override IEnumerable<RestApiKanbanSwimlane> RetrieveSwimlanes(IRequest request)
        {
            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceKey);

            if (workspace is null)
            {
                yield break;
            }

            using var context = ModelHub.CreateDbContext();
            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.WorkspaceId, workspace.Id);

            var classes = CoreHub.ObjectManager.GetObjects(query, context)
                .Select(x => x.Class)
                .Where(x => x is not null)
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Name);

            foreach (var @class in classes)
            {
                yield return new RestApiKanbanSwimlane
                {
                    Id = @class.Id.ToString(),
                    Label = @class.Name
                };
            }
        }

        /// <summary>
        /// Returns one card per object in the workspace, placed in the column matching its
        /// state and the swimlane matching its class.
        /// </summary>
        protected override IEnumerable<RestApiKanbanCard> RetrieveCards(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceKey);

            if (workspace is null)
            {
                yield break;
            }

            query = query.WhereEquals(x => x.WorkspaceId, workspace.Id);

            foreach (var obj in CoreHub.ObjectManager.GetObjects(query, context))
            {
                yield return new RestApiKanbanCard
                {
                    Id = obj.Id.ToString(),
                    Label = string.IsNullOrWhiteSpace(obj.Summary) ? obj.Key : obj.Summary,
                    Html = $"<strong>{obj.Key}</strong><br/>{obj.Summary}",
                    ColumnId = obj.State.ToString(),
                    SwimlaneId = obj.ClassId.ToString()
                };
            }
        }
    }
}
