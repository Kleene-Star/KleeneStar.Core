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
    /// Provides a fragment control for advanced search within the object view.
    /// </summary>
    [Section<SectionViewHeaderPrimary>]
    [Scope<ObjectViewFragment>]
    [Cache]
    public sealed class ObjectViewSearchFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Unique id that <see cref="ObjectViewTableFragment"/> binds its search source to.
        /// </summary>
        public static readonly string ContentId = "id_FF089423B635469592CF7663BFE1CDFC";

        /// <summary>
        /// Gets the advanced search control bound to the Objects WQL endpoint.
        /// </summary>
        public ControlAdvancedSearch Search { get; } = new ControlAdvancedSearch(ContentId)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects.Wql>().ToString())};

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectViewSearchFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Search);
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
