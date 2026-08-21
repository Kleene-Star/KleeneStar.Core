using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Class
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

            Alignment = _ => TypeToolbarItemAlignment.Right;
            Icon = _ => new IconCog();
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
                    Text = _ => "kleenestar.core:class.dropdown.label"
                },
                new ControlDropdownItemLink()
                {
                    Text = _ => "webexpress.webapp:edit.label",
                    Icon = _ => new IconPen(),
                    PrimaryAction = _ => new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge),
                },
                new ControlDropdownItemLink()
                {
                    Text = _ => "webexpress.webapp:clone.label",
                    Icon = _ => new IconClone(),
                    PrimaryAction = _ => new ActionModal("modal-form", cloneUri, TypeModalSize.ExtraLarge),
                },
                new ControlDropdownItemLink()
                {
                    Text = _ => "kleenestar.core:field.link.label",
                    Icon = _ => new IconField(),
                    Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Fields._classid_.Index>()?
                        .BindParameters(keyParameter)
                        .BindParameters(renderContext.Request)
                },
                new ControlDropdownItemLink()
                {
                    Text = _ => "kleenestar.core:form.link.label",
                    Icon = _ => new IconListFunction(),
                    //Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Classes._workspacekey_.Index>()?
                    //    .BindParameters(keyParameter)
                    // .BindParameters(renderContext.Request)
                },
                new ControlDropdownItemLink()
                {
                    Text = _ => "kleenestar.core:priority.link.label",
                    Icon = _ => new IconFlag(),
                    //Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Classes._workspacekey_.Index>()?
                    //    .BindParameters(keyParameter)
                    // .BindParameters(renderContext.Request)
                },
                new ControlDropdownItemLink()
                {
                    Text = _ => "kleenestar.core:status.link.label",
                    Icon = _ => new IconStatus(),
                    //Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Classes._workspacekey_.Index>()?
                    //    .BindParameters(keyParameter)
                    // .BindParameters(renderContext.Request)
                },
                new ControlDropdownItemLink()
                {
                    Text = _ => "kleenestar.core:workflow.link.label",
                    Icon = _ => new IconWorkflow(),
                    //Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Classes._workspacekey_.Index>()?
                    //    .BindParameters(keyParameter)
                    // .BindParameters(renderContext.Request)
                },
                new ControlDropdownItemDivider(),
                new ControlDropdownItemLink("Delete")
                {
                    Text = _ => "webexpress.webapp:delete.label",
                    Icon = _ => new IconTrash(),
                    PrimaryAction = _ => new ActionModal("modal-form", deleteUri, TypeModalSize.Default),
                    Color = _ => TypeColorText.Danger
                }
            };

            return base.Render(renderContext, visualTree, items);
        }
    }
}
