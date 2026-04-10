using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Renders the class dashboard on the content area of the class index page.
    /// Displays key metrics (forms, fields, priorities, statuses, and workflows)
    /// as <c>RestApiDashboardWidgetBigNumber</c> widgets, each linking to the
    /// corresponding configuration page.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Class._classid_.Index>]
    [Cache]
    public sealed class ClassDashboardFragment : FragmentControlRestDashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its
        /// operation. Cannot be null.
        /// </param>
        public ClassDashboardFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._classid_.Stats>();
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragment, or <c>null</c> if the
        /// fragment conditions are not met.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var classId = renderContext.Request.GetParameter<ClassIdParameter>()?.Value;
            var restUri = RestUri?.BindParameters(new ClassIdParameter(classId));

            return base.Render(renderContext, visualTree, restUri);
        }
    }
}
