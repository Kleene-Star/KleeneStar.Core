using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Status
{
    /// <summary>
    /// Represents a fragment that provides a quick filter control for REST-based workspace queries in
    /// the status view.
    /// </summary>
    [Section<SectionViewHeaderSecondary>]
    //[Policy<StatusViewPolicy>]
    [Scope<StatusViewFragment>]
    [Cache]
    public sealed class StatusViewQuickfilterFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Represents the unique identifier for the content.
        /// </summary>
        public static readonly string ContentId = "id_BF95BF57C9A34D00A9D5D9326777248E";

        /// <summary>
        /// Gets the quick filter control for REST-based workspace queries.
        /// </summary>
        public ControlRestQuickfilter Quickfilter { get; } = new ControlRestQuickfilter(ContentId)
        {
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Statuses._classid_.Quickfilter>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public StatusViewQuickfilterFragment(IFragmentContext fragmentContext)
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
