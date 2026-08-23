using KleeneStar.Core.WebParameter;
using System.Linq;
using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Dashboard
{
    /// <summary>
    /// Renders the selected dashboard on the content area of the dashboard view page using the
    /// <c>ControlDataDashboard</c> control backed by the <c>RestApiDashboard</c> REST endpoint. The
    /// board is fully editable: columns can be added, renamed, resized, recolored, reordered and
    /// deleted, and widgets can be added, reconfigured and removed, all persisted through the
    /// endpoint.
    /// </summary>
    /// <remarks>
    /// The app-specific widget types (and their i18n) live in the embedded scripts
    /// <c>Assets/js/i18n/en.js</c>, <c>Assets/js/i18n/de.js</c> and
    /// <c>Assets/js/widgets/kleenestar.js</c>. They are emitted inline into the page head, in that
    /// order (the i18n registrations must precede the widget registration that looks them up), rather
    /// than served as static assets: the core plugin's own embedded assets mount under
    /// <c>/{app}/assets/…</c>, a route the application already owns for its workspace-assets feature,
    /// so they are shadowed and never served. Inlining keeps the widgets working regardless.
    /// </remarks>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Index>]
    [Scope<global::KleeneStar.Core.WWW.Dashboard._dashboardid_.Index>]
    [Cache]
    public sealed class DashboardDetailViewFragment : FragmentControlDataDashboard
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
            // the board is the view here rather than a block among others, so it takes the
            // height it is handed instead of growing with its longest column: the widgets then
            // scroll below a menu bar that stays, so adding a column stays reachable
            Fill = _ => true;

            ServiceFactory = renderContext => DataServiceDescriptor.Data(BuildViewUri(renderContext)?.ToString());

            // enable the full board editing surface; the endpoint persists every change and reports
            // which widget types the add menu may offer
            EditableColumn = _ => true;
            MovableColumn = _ => true;
            DeletableColumn = _ => true;
            AddableColumn = _ => true;
            AddableWidget = _ => true;
            ConfigurableWidget = _ => true;
        }

        /// <summary>
        /// Renders the control as an HTML node, first injecting the app widget registration script
        /// into the page head.
        /// </summary>
        /// <param name="renderContext">The context in which the control is rendered.</param>
        /// <param name="visualTree">The visual tree representing the control's structure.</param>
        /// <returns>An HTML node representing the rendered control.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            // inline the widget registration; a head script runs after the framework script links
            // (the base widget registry and the client i18n) but before the board controller reads
            // the registry, so the app widgets are available by the time the add menu is built.
            var script = DashboardWidgetScript.Value;
            if (!string.IsNullOrEmpty(script))
            {
                visualTree.AddHeaderScript(script);
            }

            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Resolves the REST view URI for the dashboard the page shows, bound to a concrete dashboard
        /// id. The id comes from the route on the detail page (<c>/dashboard/{dashboardid}</c>); on the
        /// index page, which carries no id, it falls back to the first dashboard so the board still
        /// loads. Returns an unbound URI only when no dashboard exists at all.
        /// </summary>
        /// <param name="renderContext">The render context carrying the request.</param>
        /// <returns>The bound view URI, or the unbound URI when no dashboard is available.</returns>
        private static IUri BuildViewUri(IRenderControlContext renderContext)
        {
            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Dashboards._dashboardid_.View>();

            var dashboardId = renderContext?.Request?.GetParameter<DashboardIdParameter>()?.Value;

            if (string.IsNullOrEmpty(dashboardId))
            {
                dashboardId = CoreHub.DashboardManager
                    .GetDashboards(new Query<global::KleeneStar.Model.Entities.Dashboard>().WithPaging(0, 1))
                    .FirstOrDefault()?.Id.ToString();
            }

            return string.IsNullOrEmpty(dashboardId)
                ? uri
                : uri?.BindParameters(new DashboardIdParameter { Value = dashboardId });
        }
    }
}
