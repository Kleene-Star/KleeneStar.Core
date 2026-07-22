using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebRestApi
{
    /// <summary>
    /// Project-wide base for the object dashboard endpoint of a kind's overview tab
    /// control: a small KPI dashboard (total / active / archived) aggregating the
    /// workspace's objects of the <see cref="Kind"/>. A concrete subclass only fixes the
    /// kind it aggregates (issue, asset, …); each concrete endpoint registers at its own
    /// route, so the base must stay abstract.
    /// </summary>
    public abstract class RestApiObjectKindDashboard : RestApiDashboard
    {
        /// <summary>
        /// Gets the persisted kind key the dashboard aggregates.
        /// </summary>
        protected abstract string Kind { get; }

        /// <summary>
        /// Returns one KPI column for the total number of objects of the kind in the
        /// workspace and one each for the active and archived counts.
        /// </summary>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The dashboard columns.</returns>
        protected override IEnumerable<RestApiDashboardColumn> RetrieveColumns(IRequest request)
        {
            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceKey);

            if (workspace is null)
            {
                yield break;
            }

            using var context = ModelHub.CreateDbContext();

            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.WorkspaceId, workspace.Id)
                .WhereEquals(x => x.Kind, Kind);

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
