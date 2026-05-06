using KleeneStar.Core.WebParameter;
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
    /// Represents an add form fragment for a template.
    /// </summary>
    [Title("kleenestar.core:template.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Templates._workspacekey_.Add>]
    [Cache]
    public sealed class TemplateAddFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Gets the input control configuration for the template name field.
        /// </summary>
        public ControlFormItemInputText TemplateName { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Template.Name),
            Label = _ => "kleenestar.core:template.name.label",
            Placeholder = _ => "kleenestar.core:template.name.placeholder",
            Help = _ => "kleenestar.core:template.name.help",
            Required = _ => true
        };

        /// <summary>
        /// Gets the input control for editing the template description.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Template.Description),
            Label = _ => "kleenestar.core:template.description.label",
            Placeholder = _ => "kleenestar.core:template.description.placeholder",
            Format = _ => TypeEditTextFormat.Wysiwyg,
            Required = _ => false
        };

        /// <summary>
        /// Gets the input control for specifying the category of the template.
        /// </summary>
        public ControlFormItemInputText Category { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Template.Category),
            Label = _ => "kleenestar.core:template.category.label",
            Placeholder = _ => "kleenestar.core:template.category.placeholder",
            Help = _ => "kleenestar.core:template.category.help"
        };

        /// <summary>
        /// Gets the configuration for the class selection input used in the form.
        /// </summary>
        public ControlRestFormItemInputSelection ClassSelection { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Template.ClassId),
            Label = _ => "kleenestar.core:template.class.label",
            Placeholder = _ => "kleenestar.core:template.class.placeholder",
            Help = _ => "kleenestar.core:template.class.help",
            StickySelection = _ => true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_.Index>()
        };

        /// <summary>
        /// Gets the input selection configuration for the template state field.
        /// </summary>
        public ControlRestFormItemInputSelection TemplateState { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Template.State),
            Label = _ => "kleenestar.core:template.state.label",
            Placeholder = _ => "kleenestar.core:template.state.placeholder",
            Help = _ => "kleenestar.core:template.state.help",
            StickySelection = _ => true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Templates.State>()
        };

        /// <summary>
        /// Initializes a new instance of the TemplateAddFormFragment class for creating a new template.
        /// </summary>
        /// <remarks>
        /// This form fragment is intended for creating new templates and automatically sets the mode to Add.
        /// The associated URI points to the index endpoint for templates within the current workspace.
        /// </remarks>
        /// <param name="fragmentContext">
        /// The fragment context used to initialize the form fragment. Must not be null.
        /// </param>
        public TemplateAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(TemplateName);
            Add(Description);
            Add(Category);
            Add(ClassSelection);
            Add(TemplateState);

            Mode = _ => TypeRestFormMode.Add;
            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Templates._workspacekey_.Index>();
        }

        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            var param1 = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var p = param1?.Value;

            return base.Render(renderContext, visualTree);
        }
    }
}
