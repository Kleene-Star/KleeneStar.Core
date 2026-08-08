using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// The permission dialog of a resource: the search box, and below it the group rows with the
    /// policies each group holds.
    /// </summary>
    /// <remarks>
    /// The permission surface used to build its own search box and its own pager. It no longer
    /// does: the pager it now renders itself as a framework pagination control, and the search box
    /// it leaves to whoever hosts it, offering only the bind to attach one. So the box is
    /// contributed here — once, because every resource's dialog wants the same one — and the
    /// endpoint keeps receiving the search term it already filters on.
    /// </remarks>
    public abstract class PermissionFragment : FragmentControlDataPermission
    {
        /// <summary>
        /// Gets the search box that narrows the listed groups.
        /// </summary>
        /// <remarks>
        /// The id is derived from the surface's own id, in the same way the surface derives the id
        /// of its pager, so several dialogs on one page cannot bind each other's box.
        /// </remarks>
        public ControlSearch Search { get; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        protected PermissionFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Search = new ControlSearch($"{Id}_search")
            {
                Placeholder = _ => "kleenestar.core:permission.search.placeholder"
            };

            Bind = _ => new Binding()
                .Add(new BindSearch() { Source = Search.Id });
        }

        /// <summary>
        /// Renders the fragment as an HTML node.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>
        /// An HTML node representing the search box and the permission surface, or null when the
        /// fragment's conditions keep it off the page.
        /// </returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var surface = base.Render(renderContext, visualTree);

            return surface is null
                ? null
                : new HtmlList(Search.Render(renderContext, visualTree), surface);
        }
    }
}
