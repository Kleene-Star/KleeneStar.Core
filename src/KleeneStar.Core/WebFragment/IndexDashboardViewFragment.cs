using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Renders the selected dashboard on the content area of the dashboard view page
    /// (<see cref="global::KleeneStar.Core.WWW.Dashboard._dashboardid_.Index"/>) using the
    /// <c>ControlRestDashboard</c> control backed by the <c>RestApiDashboard</c> REST endpoint.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Dashboard._dashboardid_.Index>]
    [Cache]
    public sealed class IndexDashboardViewFragment : FragmentControlRestDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its
        /// operation. Cannot be null.
        /// </param>
        public IndexDashboardViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
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

            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Dashboards._dashboardid_.View>()?
                .BindParameters(renderContext.Request);

            return base.Render(renderContext, visualTree);
        }
    }
}
