using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Tenant
{
    /// <summary>
    /// Represents a fragment that provides a quick filter control for REST-based tenant queries in
    /// the tenant view.
    /// </summary>
    [Section<SectionViewHeaderSecondary>]
    [Scope<TenantViewFragment>]
    [Cache]
    public sealed class TenantViewQuickfilterFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Represents the unique identifier for the content.
        /// </summary>
        public static readonly string ContentId = "id_5C8A91BD37624E2A8F4E0D0B72F61E83";

        /// <summary>
        /// Gets the quick filter control for REST-based tenant queries.
        /// </summary>
        public ControlDataQuickfilter Quickfilter { get; } = new ControlDataQuickfilter(ContentId)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Tenants.Quickfilter>().ToString())
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public TenantViewQuickfilterFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Quickfilter);
        }

        /// <summary>
        /// Renders the control as an HTML node.
        /// </summary>
        /// <param name="renderContext">
        /// The context in which the control is rendered.
        /// </param>
        /// <param name="visualTree">
        /// The visual tree representing the control's structure.
        /// </param>
        /// <returns>
        /// An HTML node representing the rendered control.
        /// </returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
