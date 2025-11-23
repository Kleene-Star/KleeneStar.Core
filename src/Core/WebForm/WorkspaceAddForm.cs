using WebExpress.WebApp.WebApiControl;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebForm
{
    /// <summary>
    /// Represents a form used to add a new workspace.
    /// </summary>
    public class WorkspaceAddForm : ControlForm
    {
        /// <summary>
        /// Returns the input text control for specifying the name of the workspace.
        /// </summary>
        public ControlRestFormItemInputUnique WorkspaceName { get; } = new ControlRestFormItemInputUnique()
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
        public ControlRestFormItemInputUnique Key { get; } = new ControlRestFormItemInputUnique()
        {
            Name = "key",
            Label = "kleenestar.core:workspace.key.label",
            Placeholder = "kleenestar.core:workspace.key.placeholder",
            Help = "kleenestar.core:workspace.key.help",
            Required = true,
            RestUri = KleeneStar.GetUri<WWW.Api._1.Workspaces.Unique>()
        };

        public ControlFormItemInputTag Category { get; } = new ControlFormItemInputTag()
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
        /// Initializes a new instance of the class.
        /// </summary>
        public WorkspaceAddForm()
            : base("kleenestar-workspace-form-add")
        {
            Add(WorkspaceName);
            Add(Key);
            Add(Category);
            Add(Description);
            AddPrimaryButton(new ControlFormItemButtonSubmit()
            {
                Icon = new IconPlus(),
                Text = "kleenestar.core:workspace.add.submit.label"
            });
        }
    }
}
