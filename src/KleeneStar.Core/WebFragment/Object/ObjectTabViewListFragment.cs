using KleeneStar.Core.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Provides a list view fragment for displaying objects with integrated search,
    /// filtering, and pagination capabilities. Rendered as a view item inside the
    /// <see cref="ObjectTabViewFragment"/> tab template and as the content of the
    /// standalone tab template.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    [Scope<ObjectTabViewFragment>]
    [Order(2)]
    [Cache]
    public sealed class ObjectTabViewListFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the list control rendering the objects as a vertical frame list. 
        /// </summary>
        public ListDetailControl List { get; } = new ListDetailControl()
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.List>().ToString())
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabViewListFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconList();
            Title = _ => "kleenestar.core:view.list.title";
            List.Bind = _ => new Binding()
                .Add(new BindSearch() { Source = ObjectTabViewSearchFragment.ContentId })
                .Add(new BindFilter())
                .Add(new BindPaging() { Source = ObjectTabViewPaginationFragment.ContentId });

            Add(List);
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
