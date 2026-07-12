using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Identity
{
    /// <summary>
    /// Represents a fragment that provides a quick filter control for REST-based identity queries in
    /// the identity view.
    /// </summary>
    [Section<SectionViewHeaderSecondary>]
    [Scope<IdentityViewFragment>]
    [Cache]
    public sealed class IdentityViewQuickfilterFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Represents the unique identifier for the content.
        /// </summary>
        public static readonly string ContentId = "id_6D43F8A21C9E4D2F8B175A0F4CE9B12C";

        /// <summary>
        /// Gets the quick filter control for REST-based identity queries.
        /// </summary>
        public ControlRestQuickfilter Quickfilter { get; } = new ControlRestQuickfilter(ContentId)
        {
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Identities.Quickfilter>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public IdentityViewQuickfilterFragment(IFragmentContext fragmentContext)
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
