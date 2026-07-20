using KleeneStar.Model.Entities;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebFragment;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Tab template for the <see cref="ObjectViewType.Table"/> views of the workspace objects
    /// index: the classic object view. Its content is composed automatically from the fragments
    /// scoped to this fragment — the switchable table, tile and list items
    /// (<see cref="ObjectTabViewTableFragment"/>, <see cref="ObjectTabViewTileFragment"/>,
    /// <see cref="ObjectTabViewListFragment"/>) together with the search, quickfilter and
    /// pagination chrome (<see cref="ObjectTabViewSearchFragment"/>,
    /// <see cref="ObjectTabViewQuickfilterFragment"/>, <see cref="ObjectTabViewPaginationFragment"/>).
    /// </summary>
    [Section<SectionTabTemplatePrimary>]
    [Scope<ObjectTabViewTemplateFragment>]
    [Order(0)]
    [Cache]
    public sealed class ObjectTabViewFragment : FragmentControlView, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Layout = _ => WebExpress.WebUI.WebControl.TypeLayoutView.ToggleGroup;
        }
    }
}
