using KleeneStar.Core.WebForm;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.WorkspaceManager
{
    /// <summary>
    /// Represents a page that provides a form for adding a new workspace within the application.
    /// </summary>
    [WebIcon<IconPlus>]
    [Title("kleenestar.core:workspace.edit.label")]
    [Scope<IScopeGeneral>]
    public sealed class Add : IPage<VisualTreeWebApp>
    {
        /// <summary>
        /// Returns the form used to add a new workspace.
        /// </summary>
        public WorkspaceAddForm Form { get; } = new WorkspaceAddForm();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Add()
        {
            Form.ProcessForm += OnProcessForm;
        }

        /// <summary>
        /// Handles the processing of a form event triggered by the specified <see cref="ControlFormEventFormProcess"/>
        /// object.
        /// </summary>
        /// <param name="argument">The form process event object containing the data and context required for processing the form.</param>
        private void OnProcessForm(ControlFormEventFormProcess argument)
        {
            var name = argument.GetValue<ControlFormInputValueString>(Form.WorkspaceName);
            var key = argument.GetValue<ControlFormInputValueString>(Form.Key);
            var category = argument.GetValue<ControlFormInputValueStringList>(Form.Category);
            var description = argument.GetValue<ControlFormInputValueString>(Form.Description);

            var workspace = new WebWorkspace.Workspace()
            {
                Name = name.Text,
                Key = key.Text,
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
