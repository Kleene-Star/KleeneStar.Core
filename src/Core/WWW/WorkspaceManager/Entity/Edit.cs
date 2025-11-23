using KleeneStar.Core.WebForm;
using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.WorkspaceManager.Entity
{
    /// <summary>
    /// Represents the page for creating or editing a workspace within the web application. Provides access to the
    /// workspace edit form and handles form processing and rendering.
    /// </summary>
    [WebIcon<IconPencil>]
    [Title("kleenestar.core:workspace.edit.label")]
    [Scope<IScopeGeneral>]
    public sealed class Edit : IPage<VisualTreeWebApp>
    {
        /// <summary>
        /// Returns the form used to add a new workspace.
        /// </summary>
        public WorkspaceEditForm Form { get; } = new WorkspaceEditForm();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Edit()
        {
            Form.InitializeForm += OnInitializeForm;
            Form.ProcessForm += OnProcessForm;
        }

        /// <summary>
        /// Initializes the form with workspace-specific data based on the 
        /// provided initialization context.
        /// </summary>
        /// <param name="argument">
        /// event argument containing the form initialization context and 
        /// request parameters.
        /// </param>
        private void OnInitializeForm(ControlFormEventFormInitialize argument)
        {
            var workspaceKey = argument.Context.Request.GetParameter<KeyParameter>();
            var workspace = KleeneStar.WorkspaceManager.GetWorkspaceByKey(workspaceKey.Value);

            if (workspace is not null)
            {
                argument.SetValue(Form.WorkspaceName, new ControlFormInputValueString(workspace.Name));
                //workspace.Category = argument.GetValue<ControlFormInputValueStringList>(Form.Category);
                //workspace.Description = argument.GetValue<ControlFormInputValueString>(Form.Description);
            }
        }

        /// <summary>
        /// Handles the processing of a form event triggered by the specified <see cref="ControlFormEventFormProcess"/>
        /// object.
        /// </summary>
        /// <param name="argument">The form process event object containing the data and context required for processing the form.</param>
        private void OnProcessForm(ControlFormEventFormProcess argument)
        {
            var name = argument.GetValue<ControlFormInputValueString>(Form.WorkspaceName);
            var category = argument.GetValue<ControlFormInputValueStringList>(Form.Category);
            var description = argument.GetValue<ControlFormInputValueString>(Form.Description);

            var workspace = new WebWorkspace.Workspace()
            {
                //Text = name.Value,
                Name = name.Text,
                Description = description.Text,
                //Categories = category.Values
            };

            KleeneStar.WorkspaceManager.AddWorkspace(workspace);
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            visualTree.Content.MainPanel.AddPrimary(Form);
        }
    }
}
