using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebFragment;

namespace KleeneStar.Core.WebFragment.Object.Assets
{
    /// <summary>
    /// Classic asset view hosted by the <see cref="AssetTabViewTemplateFragment"/> tab
    /// template. Its content is composed automatically from the switchable table, tile and
    /// list items (<see cref="AssetTabViewTableFragment"/>,
    /// <see cref="AssetTabViewTileFragment"/>, <see cref="AssetTabViewListFragment"/>)
    /// together with the search, quickfilter and pagination chrome scoped to this fragment.
    /// </summary>
    [Section<SectionTabTemplatePrimary>]
    [Scope<AssetTabViewTemplateFragment>]
    [Order(0)]
    [Cache]
    public sealed class AssetTabViewFragment : FragmentControlView, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public AssetTabViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Layout = _ => WebExpress.WebUI.WebControl.TypeLayoutView.ToggleGroup;
        }
    }
}
