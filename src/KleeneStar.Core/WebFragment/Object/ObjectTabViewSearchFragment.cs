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
    /// Provides a fragment control for advanced search within the object view. Rendered
    /// as a view header inside the <see cref="ObjectTabViewFragment"/> tab template and the
    /// standalone tab template.
    /// </summary>
    [Section<SectionViewHeaderPrimary>]
    [Scope<ObjectTabViewFragment>]
    [Cache]
    public sealed class ObjectTabViewSearchFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Unique id that <see cref="ObjectTabViewTableFragment"/> binds its search source to.
        /// </summary>
        public static readonly string ContentId = "id_FF089423B635469592CF7663BFE1CDFC";

        /// <summary>
        /// Gets the advanced search control bound to the Objects WQL endpoint.
        /// </summary>
        public ControlAdvancedSearch Search { get; } = new ControlAdvancedSearch(ContentId)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects.Wql>().ToString())
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabViewSearchFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Search);
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
