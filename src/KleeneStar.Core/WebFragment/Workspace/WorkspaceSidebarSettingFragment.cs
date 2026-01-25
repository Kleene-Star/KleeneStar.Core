using KleeneStar.Core.WebParameter.Workspace;
using KleeneStar.Core.Workspace;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workspace
{
    /// <summary>
    /// Represents a sidebar item link fragment that displays the 'All' quick filter option in the workspace sidebar.
    /// </summary>
    [Section<SectionSidebarToolbarPrimary>]
    [Scope<WWW.Workspaces._key_.Index>]
    [Cache]
    public sealed class WorkspaceSidebarSettingFragment : FragmentControlToolbarItemDropdown
    {
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        /// <param name="workspaceManager">
        /// The workspace manager used to retrieve workspace information. Cannot be null.
        /// </param>
        public WorkspaceSidebarSettingFragment(IFragmentContext fragmentContext, IWorkspaceManager workspaceManager)
            : base(fragmentContext)
        {
            _workspaceManager = workspaceManager;

            Alignment = TypeToolbarItemAlignment.Right;
            Icon = new IconCog();
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<KeyParameter>();
            var workspace = _workspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            var items = new IControlDropdownItem[]
            {
                new ControlDropdownItemHeader()
                {
                    Text = "kleenestar.core:workspace.dropdown.label"
                },
                new ControlDropdownItemLink()
                {
                    Text = "webexpress.webapp:edit.label",
                    Icon = new IconPencil(),
                    Modal = new ModalTarget("modal-form"),
                    Uri = CoreHub.GetUri<WWW.Workspaces._key_.Edit>()?
                        .SetParameters(keyParameter)
                },
                new ControlDropdownItemLink()
                {
                    Text = "webexpress.webapp:clone.label",
                    Icon = new IconCopy(),
                    Modal = new ModalTarget("modal-form"),
                    Uri = CoreHub.GetUri<WWW.Workspaces._key_.Clone>()?
                        .SetParameters(keyParameter)
                },
                new ControlDropdownItemLink()
                {
                    Text = "kleenestar.core:class.manage.label",
                    Icon = new IconBoxesStacked(),
                    Uri = CoreHub.GetUri<WWW.Workspaces._key_.Classes.Index>()?
                        .SetParameters(keyParameter)
                },
                new ControlDropdownItemDivider(),
                new ControlDropdownItemLink("Delete")
                {
                    Text = "webexpress.webapp:delete.label",
                    Icon = new IconTrashAlt(),
                    Modal = new ModalTarget("modal-form"),
                    Color = TypeColorText.Danger,
                    Uri = CoreHub.GetUri<WWW.Workspaces._key_.Delete>()?
                        .SetParameters(keyParameter)
                }
            };

            return base.Render(renderContext, visualTree, items);
        }
    }
}
