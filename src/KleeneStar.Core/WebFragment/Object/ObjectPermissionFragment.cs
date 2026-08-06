using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// The permission dialog of a single object, opened from the object's more menu.
    /// </summary>
    /// <remarks>
    /// The menu entry and the page it opens existed already, but the page carried nothing: the
    /// dialog came up empty. This is the surface it was meant to show.
    /// </remarks>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Permission>]
    [Cache]
    public sealed class ObjectPermissionFragment : FragmentControlDataPermission
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectPermissionFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            this.DataService<global::KleeneStar.Core.WWW.Api._1_.Issue._objectkey_.Permission>();
            this.GroupsService<global::KleeneStar.Core.WWW.Api._1_.Issue._objectkey_.PermissionGroups>();
            this.PoliciesService<global::KleeneStar.Core.WWW.Api._1_.Issue._objectkey_.PermissionPolicies>();
        }
    }
}
