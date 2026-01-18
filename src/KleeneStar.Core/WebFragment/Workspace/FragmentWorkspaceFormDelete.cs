using KleeneStar.Core.WebParameter.Workspace;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workspace
{
    /// <summary>
    /// Represents a delete form fragment for a workspace.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<WWW.Workspace.Id.Delete>]
    [Cache]
    public sealed class FragmentWorkspaceFormDelete : FragmentControlRestFormDelete
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public FragmentWorkspaceFormDelete(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Mode = TypeRestFormMode.Delete;
            Uri = CoreHub.GetUri<WWW.Api._1.Workspaces.Index>();
        }

        /// <summary>
        /// Renders the control as an HTML node.
        /// </summary>
        /// <param name="renderContext">
        /// The context in which the control is rendered.
        /// </param>
        /// <param name="visualTree">
        /// The visual tree representing the control's structure.
        /// </param>
        /// <returns>
        /// An HTML node representing the rendered control.
        /// </returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var key = renderContext.Request.GetParameter<KeyParameter>();
            var id = CoreHub.WorkspaceManager.GetWorkspaceByKey(key?.Value)?
                .Id.ToString();

            return base.Render(renderContext, visualTree, Items, id);
        }
    }
}
