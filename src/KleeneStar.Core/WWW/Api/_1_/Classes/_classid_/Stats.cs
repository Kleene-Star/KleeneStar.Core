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
            var guid = Guid.TryParse(classIdParam?.Value, out var id) ? id : Guid.Empty;

            var formCount = CoreHub.FormManager.GetForms(classIdParam).Count();
            var fieldCount = CoreHub.FieldManager.GetFields(classIdParam).Count();
            var priorityCount = CoreHub.PriorityManager.GetPriorities(classIdParam).Count();
            var statusCount = CoreHub.StatusManager.GetStatuses(classIdParam).Count();
            var workflowCount = CoreHub.WorkflowManager.GetWorkflows(classIdParam).Count();

            var formsHref = CoreHub.GetUri<global::KleeneStar.Core.WWW.Forms._classid_.Index>()
                ?.BindParameters(new ClassIdParameter(guid))
                ?.ToString();
            var fieldsHref = CoreHub.GetUri<global::KleeneStar.Core.WWW.Fields._classid_.Index>()
                ?.BindParameters(new ClassIdParameter(guid))
                ?.ToString();
            var prioritiesHref = CoreHub.GetUri<global::KleeneStar.Core.WWW.Priorities._classid_.Index>()
                ?.BindParameters(new ClassIdParameter(guid))
                ?.ToString();
            var statusesHref = CoreHub.GetUri<global::KleeneStar.Core.WWW.Statuses._classid_.Index>()
                ?.BindParameters(new ClassIdParameter(guid))
                ?.ToString();
            var workflowsHref = CoreHub.GetUri<global::KleeneStar.Core.WWW.Workflows._classid_.Index>()
                ?.BindParameters(new ClassIdParameter(guid))
                ?.ToString();

            yield return new RestApiDashboardColumn
            {
                Id = "kpi",
                Size = "12",
                Widgets =
                [
                    new RestApiDashboardWidgetBigNumber
                    {
                        Id = "forms",
                        Value = formCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.forms.label"),
                        Color = "primary",
                        Params = new Dictionary<string, string> { ["href"] = formsHref }
                    },
                    new RestApiDashboardWidgetBigNumber
                    {
                        Id = "fields",
                        Value = fieldCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.fields.label"),
                        Color = "primary",
                        Params = new Dictionary<string, string> { ["href"] = fieldsHref }
                    },
                    new RestApiDashboardWidgetBigNumber
                    {
                        Id = "priorities",
                        Value = priorityCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.priorities.label"),
                        Color = "primary",
                        Params = new Dictionary<string, string> { ["href"] = prioritiesHref }
                    },
                    new RestApiDashboardWidgetBigNumber
                    {
                        Id = "statuses",
                        Value = statusCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.statuses.label"),
                        Color = "primary",
                        Params = new Dictionary<string, string> { ["href"] = statusesHref }
                    },
                    new RestApiDashboardWidgetBigNumber
                    {
                        Id = "workflows",
                        Value = workflowCount.ToString(),
                        Label = I18N.Translate(request, "kleenestar.core:class.dashboard.workflows.label"),
                        Color = "primary",
                        Params = new Dictionary<string, string> { ["href"] = workflowsHref }
                    }
                ]
            };
        }
    }
}
