using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a sidebar item link fragment that displays the 'Forms' link in the class sidebar.
    /// </summary>
    [Section<SectionSidebarPrimary>]
    [Scope<WWW.Classes._workspacekey_._classid_.Index>]
    [Cache]
    public sealed class ClassSidebarFormLinkFragment : FragmentControlSidebarItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public ClassSidebarFormLinkFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Icon = new IconRectangleList();
            Text = "kleenestar.core:form.link.label";
            //Uri = CoreHub.GetUri<WWW.Classes._workspacekey_._classid_.Forms.Index>();
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var uri = renderContext.Request.Uri.PathSegments;
            var targetUri = Uri?.BindParameters(renderContext.Request).PathSegments;

            Active = uri?.ToString() == targetUri?.ToString()
                ? TypeActive.Active
                : TypeActive.None;

            return base.Render(renderContext, visualTree);
        }
    }
}
