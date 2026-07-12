using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Dropdown item in the object headline 'more' overflow menu that opens the object-tree
    /// organize dialog for the current object's workspace, in which its objects are shown as a
    /// tree and can be re-parented by drag and drop.
    /// </summary>
    /// <remarks>
    /// The dialog is a workspace-scoped page (<c>Objects/${workspacekey}/organize</c>); the
    /// workspace is resolved from the current object. The dialog content is fetched into the modal
    /// on demand and its inline scripts would not run there, so this fragment loads the persistence
    /// controller (<c>assets/js/objectmovetree.js</c>) onto the parent page during render.
    /// </remarks>
    [Section<SectionHeadlineMorePrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Cache]
    public sealed class ObjectItemOrganizeMoreFragment : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation.
        /// Cannot be null.
        /// </param>
        public ObjectItemOrganizeMoreFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "kleenestar.core:workspace.organize.title";
            Icon = _ => new IconSitemap();
            // target the body-level page-modal rendered by ObjectOrganizeModalFragment; a form modal
            // would inject only <form> children and leave the tree dialog empty.
            PrimaryAction = renderContext => new ActionModal
            (
                "modal-organize",
                ResolveOrganizeUri(renderContext),
                TypeModalSize.ExtraLarge
            );
        }

        /// <summary>
        /// Convert the fragment to HTML. Loads the object-move-tree client controller onto the
        /// current (parent) page before emitting the dropdown item, and suppresses the item when
        /// the object (and thus its workspace) cannot be resolved.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragment, or <c>null</c> when suppressed.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (ResolveOrganizeUri(renderContext) is null)
            {
                return null;
            }

            visualTree.AddHeaderScriptLink(renderContext.PageContext.ApplicationContext.Route.Concat("assets/js/objectmovetree.js").ToString());

            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Resolves the organize dialog URI for the current object's workspace, or
        /// <see langword="null"/> when the object or its workspace cannot be resolved.
        /// </summary>
        /// <param name="renderContext">The render context carrying the object key parameter.</param>
        /// <returns>The bound organize dialog URI, or <see langword="null"/>.</returns>
        private static WebExpress.WebCore.WebUri.IUri ResolveOrganizeUri(IRenderControlContext renderContext)
        {
            var objectKey = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            var @object = CoreHub.ObjectManager.GetObjectByKey(objectKey);
            var workspace = @object is null ? null : CoreHub.WorkspaceManager.GetWorkspace(@object.WorkspaceId);

            if (workspace is null)
            {
                return null;
            }

            return CoreHub.GetUri<global::KleeneStar.Core.WWW.Objects._workspacekey_.Organize>()?
                .BindParameters(new WorkspaceKeyParameter(workspace.Key));
        }
    }
}
