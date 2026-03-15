using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
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
    /// Represents a sidebar item link fragment that displays the 'All' quick filter option in the workspace sidebar.
    /// </summary>
    [Section<SectionSidebarToolbarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Class._classid_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Fields._classid_.Index>]
    [Cache]
    public sealed class ClassItemSidebarSettingFragment : FragmentControlToolbarItemDropdown
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
        public ClassItemSidebarSettingFragment(IFragmentContext fragmentContext, IWorkspaceManager workspaceManager)
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
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var editUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Class._classid_.Edit>()?
                .BindParameters(keyParameter)
                .BindParameters(renderContext.Request);
            var cloneUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Class._classid_.Clone>()?
                .BindParameters(keyParameter)
                .BindParameters(renderContext.Request);
            var deleteUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Class._classid_.Delete>()?
                .BindParameters(keyParameter)
                .BindParameters(renderContext.Request);

            var items = new IControlDropdownItem[]
            {
                new ControlDropdownItemHeader()
                {
                    Text = "kleenestar.core:class.dropdown.label"
                },
                new ControlDropdownItemLink()
                {
                    Text = "webexpress.webapp:edit.label",
                    Icon = new IconPencil(),
                    PrimaryAction = new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge),
                },
                new ControlDropdownItemLink()
                {
                    Text = "webexpress.webapp:clone.label",
                    Icon = new IconCopy(),
                    PrimaryAction = new ActionModal("modal-form", cloneUri, TypeModalSize.ExtraLarge),
                },
                new ControlDropdownItemLink()
                {
                    Text = "kleenestar.core:field.link.label",
                    Icon = new IconList(),
                    Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Fields._classid_.Index>()?
                        .BindParameters(keyParameter)
                        .BindParameters(renderContext.Request)
                },
                new ControlDropdownItemLink()
                {
                    Text = "kleenestar.core:form.link.label",
                    Icon = new IconRectangleList(),
                    //Uri = CoreHub.GetUri<WWW.Classes._workspacekey_.Index>()?
                    //    .BindParameters(keyParameter)
                    // .BindParameters(renderContext.Request)
                },
                new ControlDropdownItemLink()
                {
                    Text = "kleenestar.core:priority.link.label",
                    Icon = new IconFlag(),
                    //Uri = CoreHub.GetUri<WWW.Classes._workspacekey_.Index>()?
                    //    .BindParameters(keyParameter)
                    // .BindParameters(renderContext.Request)
                },
                new ControlDropdownItemLink()
                {
                    Text = "kleenestar.core:workflow.link.label",
                    Icon = new IconDiagramProject(),
                    //Uri = CoreHub.GetUri<WWW.Classes._workspacekey_.Index>()?
                    //    .BindParameters(keyParameter)
                    // .BindParameters(renderContext.Request)
                },
                new ControlDropdownItemDivider(),
                new ControlDropdownItemLink("Delete")
                {
                    Text = "webexpress.webapp:delete.label",
                    Icon = new IconTrashAlt(),
                    PrimaryAction = new ActionModal("modal-form", deleteUri, TypeModalSize.Default),
                    Color = TypeColorText.Danger
                }
            };

            return base.Render(renderContext, visualTree, items);
        }
    }
}
