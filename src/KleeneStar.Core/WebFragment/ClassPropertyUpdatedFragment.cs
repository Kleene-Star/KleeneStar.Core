using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a property fragment that displays the last updated date of a class in the detail view.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Class._classid_.Index>]
    [Cache]
    public sealed class ClassPropertyUpdatedFragment : FragmentControlAttribute
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation.
        /// Cannot be null.
        /// </param>
        public ClassPropertyUpdatedFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Key = "kleenestar.core:class.updated.label";
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var classId = renderContext.Request.GetParameter<ClassIdParameter>();
            var @class = CoreHub.ClassManager.GetClass(classId);

            return base.Render(renderContext, visualTree, Key, @class?.Updated.ToString("yyyy-MM-dd"), Uri, Icon);
        }
    }
}
