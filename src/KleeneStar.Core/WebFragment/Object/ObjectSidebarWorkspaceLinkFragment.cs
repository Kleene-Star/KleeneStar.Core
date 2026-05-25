using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Sidebar link that takes the user back to the workspace-level object overview.
    /// </summary>
    [Section<SectionSidebarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Order(10)]
    [Cache]
    public sealed class ObjectSidebarWorkspaceLinkFragment : FragmentControlSidebarItemLink
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the workspace
        /// key from the current object.</param>
        public ObjectSidebarWorkspaceLinkFragment(IFragmentContext fragmentContext, IObjectManager objectManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;

            Icon = _ => new IconHouse();
            Text = _ => "kleenestar.core:object.sidebar.workspace.label";
            Uri = renderContext => GetUri(renderContext);
        }

        /// <summary>
        /// Renders the link.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }

        private IUri GetUri(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            var @object = _objectManager.GetObjectByKey(keyParameter?.Value);

            if (@object?.Workspace?.Key is null)
            {
                return null;
            }

            return CoreHub.GetUri<global::KleeneStar.Core.WWW.Objects._workspacekey_.Index>()
                .BindParameters(new WorkspaceKeyParameter(@object.Workspace.Key));
        }
    }
}
