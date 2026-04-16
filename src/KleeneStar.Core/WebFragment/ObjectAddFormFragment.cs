using KleeneStar.Core.WWW.Api._1_.Objects;
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
    /// Represents a multi-step wizard fragment for creating a new object.
    /// </summary>
    /// <remarks>
    /// The wizard guides the user through three steps:
    /// <list type="number">
    ///   <item>Workspace and Class Selection — a cascading control for choosing a workspace and class.</item>
    ///   <item>Template Selection — a tile control for choosing an object template.</item>
    ///   <item>Object Properties — standard form inputs for specifying title and summary.</item>
    /// </list>
    /// </remarks>
    [Title("kleenestar.core:object.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Objects._workspacekey_.Add>]
    [Cache]
    public sealed class ObjectAddFormFragment : FragmentControlRestWizard
    {
        /// <summary>
        /// Gets the cascading input control for selecting a workspace and class.
        /// </summary>
        public ControlFormItemInputCascading WorkspaceClassSelection { get; } = new()
        {
            Name = "WorkspaceClass",
            Label = "kleenestar.core:object.workspaceclass.label",
            Help = "kleenestar.core:object.workspaceclass.help",
            Placeholder = "kleenestar.core:object.workspaceclass.placeholder",
            Required = true
        };

        /// <summary>
        /// Gets the tile input control for selecting an object template.
        /// </summary>
        public ControlFormItemInputTile TemplateSelection { get; } = new()
        {
            Name = "Template",
            Label = "kleenestar.core:object.template.label",
            Help = "kleenestar.core:object.template.help",
            Required = true
        };

        /// <summary>
        /// Gets the input text control for specifying the summary of the object.
        /// </summary>
        public ControlRestFormItemInputUnique Summary { get; } = new()
        {
            Name = nameof(Object.Summary),
            Label = "kleenestar.core:object.summary.label",
            Placeholder = "kleenestar.core:object.summary.placeholder",
            Help = "kleenestar.core:object.summary.help",
            Required = true,
        };

        /// <summary>
        /// Gets the input text control for specifying the description of the object.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Object.Description),
            Label = "kleenestar.core:object.description.label",
            Placeholder = "kleenestar.core:object.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            var step1 = new ControlRestWizardPage("step-workspace-class");
            step1.Add(WorkspaceClassSelection);

            var step2 = new ControlRestWizardPage("step-template");
            step2.Add(TemplateSelection);

            var step3 = new ControlRestWizardPage("step-properties");
            step3.Add(Summary);
            step3.Add(Description);

            Add(step1, step2, step3);

            Mode = TypeRestFormMode.Add;
            RestUri = CoreHub.GetUri<Index>();
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
    }
}
