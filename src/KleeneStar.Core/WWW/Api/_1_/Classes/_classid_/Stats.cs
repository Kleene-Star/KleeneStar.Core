using KleeneStar.Core.WebParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Classes._classid_
{
    /// <summary>
    /// Provides a REST API endpoint that returns the KPI dashboard for a specific class.
    /// Displays the total count of forms, fields, priorities, statuses, and workflows
    /// as <c>RestApiDashboardWidgetBigNumber</c> widgets, and links to the respective
    /// configuration pages.
    /// </summary>
    [Title("kleenestar.core:class.dashboard.stats.header")]
    [Cache]
    public sealed class Stats : RestApiDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Stats()
        {
        }

        /// <summary>
        /// Retrieves the column and widget layout for the class dashboard.
        /// Each column widget is a <c>RestApiDashboardWidgetBigNumber</c> representing
        /// the total count of a class configuration entity (forms, fields, priorities,
        /// statuses, and workflows).
        /// </summary>
        /// <param name="request">The current HTTP request. Cannot be null.</param>
        /// <returns>
        /// An enumerable of dashboard columns containing the KPI widgets for the class.
        /// </returns>
        protected override IEnumerable<RestApiDashboardColumn> RetrieveColumns(IRequest request)
        {
            var classIdParam = request.GetParameter<ClassIdParameter>();
            var classId = Guid.TryParse(classIdParam?.Value, out var guid) ? guid : Guid.Empty;
            if (classId == Guid.Empty)
            {
                yield break;
            }

            var formCount = CoreHub.FormManager.GetForms(classIdParam).Count();
            var fieldCount = CoreHub.FieldManager.GetFields(classIdParam).Count();
            var priorityCount = CoreHub.PriorityManager.GetPriorities(classIdParam).Count();
            var statusCount = CoreHub.StatusManager.GetStatuses(classIdParam).Count();
            var workflowCount = CoreHub.WorkflowManager.GetWorkflows(classIdParam).Count();

            yield return new RestApiDashboardColumn
            {
                Id = "kpi",
                Size = "25%",
                Label = "Fields",
                Widgets =
                [
                    new RestApiDashboardWidgetBigNumber()
                    {
                        Value = fieldCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.fields.label"),
                        Color = "#76522A",
                        Movable = false
                    },
                    new RestApiDashboardWidgetChart()
                    {
                        Color = "#ff5423",
                        Movable = false
                    }
                ]
            };

            yield return new RestApiDashboardColumn
            {
                Id = "kpi",
                Size = "25%",
                Label = "Forms",
                Widgets =
                [
                    new RestApiDashboardWidgetBigNumber()
                    {
                        Value = formCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.forms.label"),
                        Color = "#A2B284",
                        Movable = false
                    }
                ]
            };

            yield return new RestApiDashboardColumn
            {
                Id = "kpi",
                Size = "25%",
                Label = "Priorities",
                Widgets =
                [
                    new RestApiDashboardWidgetBigNumber()
                    {
                        Value = priorityCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.priorities.label"),
                        Color = "#3273A3",
                        Movable = false
                    }
                ]
            };

            yield return new RestApiDashboardColumn
            {
                Id = "kpi",
                Size = "25%",
                Label = "Workflows",
                Widgets =
                [
                    new RestApiDashboardWidgetBigNumber()
                    {
                        Value = statusCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.statuses.label"),
                        Color = "#628811",
                        Movable = false
                    },
                    new RestApiDashboardWidgetBigNumber()
                    {
                        Value = workflowCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.workflows.label"),
                        Color = "#26AA8",
                        Movable = false
                    }
                ]
            };
        }
    }
}
