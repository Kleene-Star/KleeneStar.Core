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
    /// Represents a configure form fragment for a field with tabbed layout.
    /// Provides tabs for Cardinality, Validation, Options, Filter objects, Workflow, and Priority.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Field._fieldid_.Configure>]
    [Cache]
    public sealed class FieldConfigureFormFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Gets the input selection control for the cardinality.
        /// </summary>
        public ControlRestFormItemInputSelection CardinalitySelection { get; } = new()
        {
            Name = nameof(Model.Entities.Field.Cardinality),
            Label = "kleenestar.core:field.cardinality.label",
            Placeholder = "kleenestar.core:field.cardinality.placeholder",
            Help = "kleenestar.core:field.cardinality.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.Cardinality>()
        };

        /// <summary>
        /// Gets the input tag control for validation rules.
        /// </summary>
        public ControlFormItemInputTag ValidationRules { get; } = new()
        {
            Name = nameof(Model.Entities.Field.ValidationRules),
            Label = "kleenestar.core:field.validationrules.label",
            Placeholder = "kleenestar.core:field.validationrules.placeholder",
            Help = "kleenestar.core:field.validationrules.help"
        };

        /// <summary>
        /// Gets the input text control for specifying the default specification.
        /// </summary>
        public ControlFormItemInputTextBox DefaultSpec { get; } = new()
        {
            Name = nameof(Model.Entities.Field.DefaultSpec),
            Label = "kleenestar.core:field.defaultspec.label",
            Placeholder = "kleenestar.core:field.defaultspec.placeholder",
            Help = "kleenestar.core:field.defaultspec.help",
            Required = false
        };

        /// <summary>
        /// Gets the input text control for specifying the WQL filter expression.
        /// </summary>
        public ControlFormItemInputTextBox FilterWql { get; } = new()
        {
            Name = "FilterWql",
            Label = "kleenestar.core:field.configure.wql.label",
            Placeholder = "kleenestar.core:field.configure.wql.placeholder",
            Required = false
        };

        /// <summary>
        /// Gets the input selection control for the field type.
        /// </summary>
        public ControlRestFormItemInputSelection FieldTypeSelection { get; } = new()
        {
            Name = nameof(Model.Entities.Field.FieldType),
            Label = "kleenestar.core:field.fieldtype.label",
            Placeholder = "kleenestar.core:field.fieldtype.placeholder",
            Help = "kleenestar.core:field.fieldtype.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.FieldType>()
        };

        /// <summary>
        /// Gets the checkbox control for the required flag.
        /// </summary>
        public ControlFormItemInputCheckbox FieldRequired { get; } = new()
        {
            Name = nameof(Model.Entities.Field.Required),
            Label = "kleenestar.core:field.required.label",
            Help = "kleenestar.core:field.required.help"
        };

        /// <summary>
        /// Gets the checkbox control for the unique flag.
        /// </summary>
        public ControlFormItemInputCheckbox FieldUnique { get; } = new()
        {
            Name = nameof(Model.Entities.Field.Unique),
            Label = "kleenestar.core:field.unique.label",
            Help = "kleenestar.core:field.unique.help"
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public FieldConfigureFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(CardinalitySelection);
            Add(ValidationRules);
            Add(DefaultSpec);
            Add(FieldTypeSelection);
            Add(FieldRequired);
            Add(FieldUnique);
            Add(FilterWql);

            Mode = TypeRestFormMode.Edit;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.Index>();
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
            var param = renderContext.Request.GetParameter<FieldIdParameter>();

            return base.Render(renderContext, visualTree, Items, param?.Value, Uri);
        }
    }
}
