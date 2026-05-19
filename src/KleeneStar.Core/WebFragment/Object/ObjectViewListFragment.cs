using KleeneStar.Core.WebControl;
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
    /// filtering, and pagination capabilities.
    /// </summary>
    [Section<SectionViewItemPrimary>]
    [Scope<ObjectViewFragment>]
    [Order(2)]
    [Cache]
    public sealed class ObjectViewListFragment : FragmentControlViewItem
    {
        /// <summary>
        /// Gets the list control rendering the objects as a vertical frame list. 
        /// </summary>
        public ListDetailControl List { get; } = new ListDetailControl()
        {
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.List>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectViewListFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = _ => new IconList();
            Title = _ => "kleenestar.core:view.list.title";
            List.Bind = _ => new Binding()
                .Add(new BindSearch() { Source = ObjectViewSearchFragment.ContentId })
                .Add(new BindFilter())
                .Add(new BindPaging() { Source = ObjectViewPaginationFragment.ContentId });

            Add(List);
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
