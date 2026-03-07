using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WWW.Workspaces._workspacekey_;
using KleeneStar.Model.Entities;
using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a edit form fragment for a workspace.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<Edit>]
    [Cache]
    public sealed class WorkspaceEditFormFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Returns the input text control for specifying the name of the workspace.
        /// </summary>
        public ControlRestFormItemInputUnique WorkspaceName { get; } = new()
        {
            Name = nameof(Workspace.Name),
            Label = "kleenestar.core:workspace.name.label",
            Placeholder = "kleenestar.core:workspace.name.placeholder",
            Help = "kleenestar.core:workspace.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<WWW.Api._1_.Workspaces.UniqueName>()
        };

        /// <summary>
        /// Returns the input text control for specifying the key of the workspace.
        /// </summary>
        public ControlRestFormItemInputUnique Key { get; } = new()
        {
            Name = nameof(Workspace.Key),
            Label = "kleenestar.core:workspace.key.label",
            Placeholder = "kleenestar.core:workspace.key.placeholder",
            Help = "kleenestar.core:workspace.key.help",
            Required = true,
            MaxLength = 10,
            RestUri = CoreHub.GetUri<WWW.Api._1_.Workspaces.UniqueKey>()
        };

        /// <summary>
        /// Returns the input tag definition for the workspace category field.
        /// </summary>
        public ControlFormItemInputTag Category { get; } = new()
        {
            Name = nameof(Workspace.Categories),
            Label = "kleenestar.core:workspace.category.label",
            Placeholder = "kleenestar.core:workspace.category.placeholder",
            Help = "kleenestar.core:workspace.category.help"
        };

        /// <summary>
        /// Returns the input text control for specifying the description of the workspace.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Workspace.Description),
            Label = "kleenestar.core:workspace.description.label",
            Placeholder = "kleenestar.core:workspace.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public WorkspaceEditFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Key);
            Add(WorkspaceName);
            Add(Category);
            Add(Description);

            Mode = TypeRestFormMode.Edit;
            Uri = CoreHub.GetUri<WWW.Api._1_.Workspaces.Index>();
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
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            var key = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var id = CoreHub.WorkspaceManager.GetWorkspaceByKey(key?.Value)?
                .Id.ToString();

            return base.Render(renderContext, visualTree, Items, id, Uri);
        }
    }
}
