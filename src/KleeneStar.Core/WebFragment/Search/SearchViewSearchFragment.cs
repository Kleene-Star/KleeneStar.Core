using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Search
{
    /// <summary>
    /// Provides the in-view search field on the global search page. The advanced search
    /// control queries objects across every workspace via the un-scoped object WQL endpoint
    /// and drives the results table through the table's <c>BindSearch</c> binding.
    /// </summary>
    /// <remarks>
    /// This is the page-local search element that drives the results table. The application
    /// header no longer carries a standalone search field — global search is reached from the
    /// search entry below the recently opened objects in the header object dropdown
    /// (<c>ObjectDropdownControl</c>).
    /// </remarks>
    [Section<SectionViewHeaderPrimary>]
    [Scope<SearchViewFragment>]
    [Cache]
    public sealed class SearchViewSearchFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Represents the unique identifier for the search content. Referenced by the table
        /// fragment's <c>BindSearch.Source</c>.
        /// </summary>
        public static readonly string ContentId = "id_4B7E1C9A6D2F40538192A3B4C5D6E7F0";

        /// <summary>
        /// Gets the search control used to query objects across all workspaces.
        /// </summary>
        public ControlAdvancedSearch Search { get; } = new ControlAdvancedSearch(ContentId)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects.Wql>().ToString())
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public SearchViewSearchFragment(IFragmentContext fragmentContext)
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
