using System;
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
    /// Represents a sidebar header fragment that displays class-related information within 
    /// the user interface sidebar.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Class._classid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Fields._classid_.Index>]
    [Cache]
    public sealed class ClassSidebarHeaderFragment : FragmentControlSidebarItemHeader
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public ClassSidebarHeaderFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var classParameter = renderContext.Request.GetParameter<ClassIdParameter>();
            var guid = Guid.TryParse(classParameter.Value, out var id) ? id : Guid.Empty;
            var @class = CoreHub.ClassManager.GetClass(guid);

            return base.Render(renderContext, visualTree, @class?.Name);
        }
    }
}
