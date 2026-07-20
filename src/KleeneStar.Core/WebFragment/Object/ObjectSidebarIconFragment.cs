using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using System;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Represents a sidebar icon fragment for a object, providing rendering and 
    /// editing capabilities within the object sidebar.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Documents._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Blogs._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Issues._workspacekey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Cache]
    public sealed class ObjectSidebarIconFragment : FragmentControlSidebarItemIcon
    {
        private readonly IObjectManager _objectManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        /// <param name="objectManager">
        /// The workspace manager used to retrieve object information. Cannot be null.
        /// </param>
        public ObjectSidebarIconFragment(IFragmentContext fragmentContext, IObjectManager objectManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;

            // the icon is only editable on the object detail page; on the kind
            // overviews (documents, blogs, issues) it is a read-only workspace icon
            IconEdit = renderContext => IsObjectContext(renderContext);
            Icon = renderContext => GetIcon(renderContext);
            PrimaryAction = renderContext => IsObjectContext(renderContext)
                ? new ActionModal("modal-form", GetUri(renderContext))
                : null;
        }

        /// <summary>
        /// Determines whether the current request addresses an object detail page (as
        /// opposed to a workspace-scoped kind overview).
        /// </summary>
        /// <param name="renderContext">
        /// The rendering context that provides information about the current HTTP request.
        /// </param>
        /// <returns><see langword="true"/> when an object key is present in the request.</returns>
        private static bool IsObjectContext(IRenderControlContext renderContext)
        {
            return !string.IsNullOrEmpty(renderContext?.Request?.GetParameter<ObjectKeyParameter>()?.Value);
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
            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Gets the URI for the Avatar endpoint with bound request parameters.
        /// </summary>
        /// <param name="renderContext">
        /// The render control context containing the request.
        /// </param>
        /// <returns>
        /// The URI for the Avatar endpoint with bound parameters, or 
        /// <see langword="null"/> if the URI cannot be retrieved.
        /// </returns>
        private IUri GetUri(IRenderControlContext renderContext)
        {
            var objectParameter = renderContext.Request.GetParameter<ObjectKeyParameter>();
            var workspaceKey = !string.IsNullOrEmpty(objectParameter?.Value)
                ? _objectManager.GetObjectByKey(objectParameter.Value)?.Workspace?.Key
                : renderContext.Request.GetParameter<WorkspaceKeyParameter>()?.Value;

            return CoreHub.GetUri<global::KleeneStar.Core.WWW.Workspaces._workspacekey_.Avatar>()?
                .BindParameters(new WorkspaceKeyParameter(workspaceKey));
        }

        /// <summary>
        /// Retrieves the icon associated with the class specified in the current 
        /// render context.
        /// </summary>
        /// <param name="renderContext">
        /// The rendering context containing the request parameters used to identify 
        /// the class.
        /// </param>
        /// <returns>
        /// The icon for the specified class, or null if the class is not found or 
        /// does not have an associated icon.
        /// </returns>
        private IIcon GetIcon(IRenderControlContext renderContext)
        {
            var workspaceParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var objectParameter = renderContext.Request.GetParameter<ObjectKeyParameter>();

            // ensure that at least one of the required parameters is present
            if (workspaceParameter is null && objectParameter is null)
            {
                throw new InvalidOperationException("One of the parameters 'object' or 'workspace' must be set.");
            }

            if (!string.IsNullOrEmpty(objectParameter?.Value))
            {
                var @object = _objectManager.GetObjectByKey(objectParameter.Value);
                // prefer the object's own icon; fall back to the workspace icon only when unset
                return @object?.Icon ?? @object?.Workspace?.Icon;
            }

            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceParameter.Value)
                ?? throw new InvalidOperationException($"Workspace with key '{workspaceParameter.Value}' not found.");

            return workspace.Icon;
        }
    }
}
