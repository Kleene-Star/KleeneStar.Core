using KleeneStar.Core.WebParameter;
using System.Linq;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Renders the selected dashboard on the content area of the dashboard view page using the
    /// <c>ControlRestDashboard</c> control backed by the <c>RestApiDashboard</c> REST endpoint.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Index>]
    [Scope<global::KleeneStar.Core.WWW.Dashboard._dashboardid_.Index>]
    [Cache]
    public sealed class DashboardDetailViewFragment : FragmentControlRestDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its
        /// operation. Cannot be null.
        /// </param>
        public DashboardDetailViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Dashboards._dashboardid_.View>();
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragment, or <c>null</c> if the
        /// fragment conditions are not met.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var dashboardId = renderContext.Request.GetParameter<DashboardIdParameter>()?.Value
                ?? CoreHub.DashboardManager.GetDashboards(new Query<Model.Entities.Dashboard>())
                    .FirstOrDefault()?.Id.ToString();

            //var restUri = RestUri?.BindParameters(new DashboardIdParameter() { Value = dashboardId });

            return base.Render(renderContext, visualTree);
        }
    }
}
