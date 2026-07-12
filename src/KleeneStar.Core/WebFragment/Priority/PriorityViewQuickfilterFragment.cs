using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Priority
{
    /// <summary>
    /// Represents a fragment that provides a quick filter control for REST-based workspace queries in
    /// the priority view.
    /// </summary>
    [Section<SectionViewHeaderSecondary>]
    //[Policy<PriorityViewPolicy>]
    [Scope<PriorityViewFragment>]
    [Cache]
    public sealed class PriorityViewQuickfilterFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Represents the unique identifier for the content.
        /// </summary>
        public static readonly string ContentId = "id_65EEEE05CCBF4B99823A466228A47221";

        /// <summary>
        /// Gets the quick filter control for REST-based workspace queries.
        /// </summary>
        public ControlDataQuickfilter Quickfilter { get; } = new ControlDataQuickfilter(ContentId)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities._classid_.Quickfilter>().ToString())};

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public PriorityViewQuickfilterFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Quickfilter);
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
