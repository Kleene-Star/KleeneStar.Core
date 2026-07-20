using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Represents a sidebar header fragment that displays object-related information within
    /// the user interface sidebar. On the object detail page the header shows the object's
    /// summary; on the workspace-level object listing the header falls back to the
    /// workspace name.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Documents._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Blogs._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Issues._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Cache]
    public sealed class ObjectSidebarHeaderFragment : FragmentControlSidebarItemHeader
    {
        private readonly IObjectManager _objectManager;
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation.
        /// Cannot be null.
        /// </param>
        /// <param name="objectManager">
        /// The object manager used to retrieve object information. Cannot be null.
        /// </param>
        /// <param name="workspaceManager">
        /// The workspace manager used to retrieve workspace information when the fragment is
        /// rendered on a scope that only carries a workspace key. Cannot be null.
        /// </param>
        public ObjectSidebarHeaderFragment(IFragmentContext fragmentContext, IObjectManager objectManager, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _workspaceManager = workspaceManager;
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
            var objectKey = renderContext.Request.GetParameter<ObjectKeyParameter>();
            if (!string.IsNullOrEmpty(objectKey?.Value))
            {
                var @object = _objectManager.GetObjectByKey(objectKey.Value);
                return base.Render(renderContext, visualTree, @object?.Summary);
            }

            var workspaceKey = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(workspaceKey?.Value);

            return base.Render(renderContext, visualTree, workspace?.Name);
        }
    }
}
