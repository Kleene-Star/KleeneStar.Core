using KleeneStar.Model.Entities;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebSection;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Tab template for the <see cref="ObjectViewType.ScrumSprint"/> view of the workspace
    /// objects index. The sprint board content is contributed by the
    /// <see cref="ObjectTabScrumSprintFragment"/> scoped to this template.
    /// </summary>
    [Section<SectionTabViewPrimary>]
    [Scope<ObjectTabFragment>]
    [Order(4)]
    [Cache]
    public sealed class ObjectTabScrumSprintTemplateFragment : FragmentControlDataTabTemplate, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabScrumSprintTemplateFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            // icon, name and description feed the template picker of the tab control;
            // the control emits the raw values, so the i18n keys are translated here
            Icon = _ => ObjectViewType.ScrumSprint.Icon();
            Name = renderContext => I18N.Translate(renderContext, ObjectViewType.ScrumSprint.Text());
            Description = renderContext => I18N.Translate(renderContext, ObjectViewType.ScrumSprint.Description());
        }
    }
}
