using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Represents a fragment control that provides REST-driven quickfilter 
    /// functionality in the object view header.
    /// </summary>
    [Section<SectionViewHeaderSecondary>]
    [Scope<ObjectViewFragment>]
    [Cache]
    public sealed class ObjectViewQuickfilterFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Unique id for the quickfilter control.
        /// </summary>
        public static readonly string ContentId = "id_2492175069D646ACAF3C9EC1D3610EAA";

        /// <summary>
        /// Gets the REST-driven quickfilter control.
        /// </summary>
        public ControlDataQuickfilter Quickfilter { get; } = new ControlDataQuickfilter(ContentId)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Quickfilter>().ToString())};

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectViewQuickfilterFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Quickfilter);
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
