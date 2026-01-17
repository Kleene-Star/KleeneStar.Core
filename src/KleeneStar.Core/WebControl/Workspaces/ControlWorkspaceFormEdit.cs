using KleeneStar.Core.WebParameter.Workspace;
using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebControl.Workspace
{
    /// <summary>
    /// Represents a form for a workspace.
    /// </summary>
    public class ControlWorkspaceFormEdit : ControlRestFormEdit
    {
        /// <summary>
        /// Returns the input text control for specifying the name of the workspace.
        /// </summary>
        public ControlRestFormItemInputUnique WorkspaceName { get; } = new()
        {
            Name = "name",
            Label = "kleenestar.core:workspace.name.label",
            Placeholder = "kleenestar.core:workspace.name.placeholder",
            Help = "kleenestar.core:workspace.name.help",
            Required = true,
            RestUri = KleeneStar.GetUri<WWW.Api._1.Workspaces.Unique>()
        };

        /// <summary>
        /// Returns the input text control for specifying the key of the workspace.
        /// </summary>
        public ControlRestFormItemInputUnique Key { get; } = new()
        {
            Name = "key",
            Label = "kleenestar.core:workspace.key.label",
            Placeholder = "kleenestar.core:workspace.key.placeholder",
            Help = "kleenestar.core:workspace.key.help",
            Required = true,
            MaxLength = 10,
            RestUri = KleeneStar.GetUri<WWW.Api._1.Workspaces.Unique>()
        };

        /// <summary>
        /// Returns the input tag definition for the workspace category field.
        /// </summary>
        public ControlFormItemInputTag Category { get; } = new()
        {
            Name = "category",
            Label = "kleenestar.core:workspace.category.label",
            Placeholder = "kleenestar.core:workspace.category.placeholder",
            Help = "kleenestar.core:workspace.category.help"
        };

        /// <summary>
        /// Returns the input text control for specifying the description of the workspace.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = "description",
            Label = "kleenestar.core:workspace.description.label",
            Placeholder = "kleenestar.core:workspace.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Returns the submit button control used to confirm the form action.
        /// </summary>
        public ControlFormItemButtonSubmit SubmitButton { get; } = new ControlFormItemButtonSubmit()
        {
            Icon = new IconPlus(),
            Text = "kleenestar.core:workspace.add.submit.label"
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ControlWorkspaceFormEdit()
            : this("kleenestar-workspace-form")
        {

        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the form control.</param>
        public ControlWorkspaceFormEdit(string id)
            : base(id)
        {
            Enable = false;

            Add(Key);
            Add(WorkspaceName);
            Add(Category);
            Add(Description);

            //AddPrimaryButton(SubmitButton);
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
            var id = KleeneStar.WorkspaceManager.GetWorkspaceByKey(key?.Value)?
                .Id.ToString();

            return base.Render(renderContext, visualTree, Items, id);
        }
    }
}
