using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a sidebar fragment that lists all available dashboards, allowing navigation 
    /// to individual dashboard pages from the home page and the dashboard detail page.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Index>]
    [Scope<global::KleeneStar.Core.WWW.Dashboard._dashboardid_.Index>]
    [Cache]
    public sealed class DashboardSidebarListFragment : FragmentControlSidebarItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation.
        /// Cannot be null.
        /// </param>
        public DashboardSidebarListFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var dashboardIdParam = renderContext.Request.GetParameter<DashboardIdParameter>();
            var dashboards = CoreHub.DashboardManager.GetDashboards(new Query<Dashboard>());
            var list = new List<IHtmlNode>();

            foreach (var dashboard in dashboards)
            {
                var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Dashboard._dashboardid_.Index>()?
                    .BindParameters(new DashboardIdParameter(dashboard.Id));

                list.Add(new ControlSidebarItemLink($"dash-{dashboard.Id}")
                {
                    Text = dashboard.Name,
                    Uri = uri,
                    Active = string.Equals(dashboardIdParam?.Value, dashboard.Id.ToString(), StringComparison.OrdinalIgnoreCase)
                        ? TypeActive.Active
                        : TypeActive.None
                }
                    .Render(renderContext, visualTree));
            }

            return new HtmlList(list);
        }
    }
}
