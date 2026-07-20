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
    /// Tab template for the <see cref="ObjectViewType.ScrumBacklog"/> view of the workspace
    /// objects index. The backlog content is contributed by the
    /// <see cref="ObjectTabScrumBacklogFragment"/> scoped to this template.
    /// </summary>
    [Section<SectionTabViewPrimary>]
    [Scope<ObjectTabFragment>]
    [Order(5)]
    [Cache]
    public sealed class ObjectTabScrumBacklogTemplateFragment : FragmentControlDataTabTemplate, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectTabScrumBacklogTemplateFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            // icon, name and description feed the template picker of the tab control;
            // the control emits the raw values, so the i18n keys are translated here
            Icon = _ => ObjectViewType.ScrumBacklog.Icon();
            Name = renderContext => I18N.Translate(renderContext, ObjectViewType.ScrumBacklog.Text());
            Description = renderContext => I18N.Translate(renderContext, ObjectViewType.ScrumBacklog.Description());
        }
    }
}
