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
    /// REST API dashboard endpoint that aggregates objects of a workspace into a small KPI
    /// dashboard (totals + per-class breakdown).
    /// </summary>
    [Title("kleenestar.core:object.view.dashboard.title")]
    [Cache]
    public sealed class Dashboard : RestApiDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Dashboard()
        {
        }

        /// <summary>
        /// Returns one KPI column for the total number of objects in the workspace and one
        /// breakdown column per class.
        /// </summary>
        protected override IEnumerable<RestApiDashboardColumn> RetrieveColumns(IRequest request)
        {
            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceKey);

            if (workspace is null)
            {
                yield break;
            }

            using var context = ModelHub.CreateDbContext();

            // the tab views live on the issue overview, so they present the issue kind only
            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.WorkspaceId, workspace.Id)
                .WhereEquals(x => x.Kind, Model.Entities.ObjectKind.Issue);

            var objects = CoreHub.ObjectManager.GetObjects(query, context).ToList();
            var active = objects.Count(x => x.State == WorkspaceState.Active);
            var archived = objects.Count(x => x.State == WorkspaceState.Archived);

            yield return new RestApiDashboardColumn
            {
                Id = "kpi-total",
                Size = "33%",
                Label = "Total",
                Widgets =
                [
                    new RestApiDashboardWidgetBigNumber
                    {
                        Value = objects.Count.ToString(),
                        Label = "Objects",
                        Color = "#3273A3",
                        Movable = false
                    }
                ]
            };

            yield return new RestApiDashboardColumn
            {
                Id = "kpi-active",
                Size = "33%",
                Label = "Active",
                Widgets =
                [
                    new RestApiDashboardWidgetBigNumber
                    {
                        Value = active.ToString(),
                        Label = "Active",
                        Color = "#A2B284",
                        Movable = false
                    }
                ]
            };

            yield return new RestApiDashboardColumn
            {
                Id = "kpi-archived",
                Size = "33%",
                Label = "Archived",
                Widgets =
                [
                    new RestApiDashboardWidgetBigNumber
                    {
                        Value = archived.ToString(),
                        Label = "Archived",
                        Color = "#76522A",
                        Movable = false
                    }
                ]
            };
        }
    }
}
