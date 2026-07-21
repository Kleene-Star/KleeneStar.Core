using KleeneStar.Model.Entities;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Object.Assets
{
    /// <summary>
    /// Tab template for the classic asset view of the workspace asset tab control: the
    /// switchable table / list / tile object view. Its content is composed automatically
    /// from the fragments scoped to the <see cref="AssetTabViewFragment"/>.
    /// </summary>
    [Section<SectionTabViewPrimary>]
    [Scope<AssetTabFragment>]
    [Order(1)]
    [Cache]
    public sealed class AssetTabViewTemplateFragment : FragmentControlDataTabTemplate, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public AssetTabViewTemplateFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            // icon, name and description feed the template picker of the tab control;
            // the control emits the raw values, so the i18n keys are translated here
            Icon = _ => ObjectViewType.Table.Icon();
            Name = renderContext => I18N.Translate(renderContext, ObjectViewType.Table.Text());
            Description = renderContext => I18N.Translate(renderContext, ObjectViewType.Table.Description());
        }
    }
}
