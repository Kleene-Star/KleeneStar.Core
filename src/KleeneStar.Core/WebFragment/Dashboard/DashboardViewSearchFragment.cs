using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Dashboard
{
    /// <summary>
    /// Represents a fragment that provides advanced search functionality within a dashboard view.
    /// </summary>
    [Section<SectionViewHeaderPrimary>]
    [Scope<DashboardViewFragment>]
    [Cache]
    public sealed class DashboardViewSearchFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Represents the unique identifier for the content used in the application.
        /// </summary>
        public static readonly string ContentId = "id_1B00598296C943688C784B91219F9475";

        /// <summary>
        /// Gets the search control used to query and filter data.
        /// </summary>
        public ControlAdvancedSearch Search { get; } = new ControlAdvancedSearch(ContentId)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Dashboards.Wql>().ToString())};

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public DashboardViewSearchFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Search);
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
