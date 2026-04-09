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
    /// Represents a edit form fragment for a field.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Field._fieldid_.Edit>]
    [Cache]
    public sealed class FieldEditFormFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the field.
        /// </summary>
        public ControlRestFormItemInputUnique FieldName { get; } = new()
        {
            Name = nameof(Model.Entities.Field.Name),
            Label = "kleenestar.core:field.name.label",
            Placeholder = "kleenestar.core:field.name.placeholder",
            Help = "kleenestar.core:field.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.UniqueName>()
        };

        /// <summary>
        /// Gets the input text control for specifying the description of the field.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.Field.Description),
            Label = "kleenestar.core:field.description.label",
            Placeholder = "kleenestar.core:field.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Gets the input text control for specifying the help text of the field.
        /// </summary>
        public ControlFormItemInputText HelpText { get; } = new()
        {
            Name = nameof(Model.Entities.Field.HelpText),
            Label = "kleenestar.core:field.helptext.label",
            Placeholder = "kleenestar.core:field.helptext.placeholder",
            Required = false
        };

        /// <summary>
        /// Gets the input text control for specifying the placeholder of the field.
        /// </summary>
        public ControlFormItemInputText FieldPlaceholder { get; } = new()
        {
            Name = nameof(Model.Entities.Field.Placeholder),
            Label = "kleenestar.core:field.placeholder.label",
            Placeholder = "kleenestar.core:field.placeholder.placeholder",
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
        /// Gets the checkbox control for the required flag.
        /// </summary>
        public ControlFormItemInputCheck FieldRequired { get; } = new()
        {
            Name = nameof(Model.Entities.Field.Required),
            Label = "kleenestar.core:field.required.label",
            Help = "kleenestar.core:field.required.help"
        };

        /// <summary>
        /// Gets the checkbox control for the unique flag.
        /// </summary>
        public ControlFormItemInputCheck FieldUnique { get; } = new()
        {
            Name = nameof(Model.Entities.Field.Unique),
            Label = "kleenestar.core:field.unique.label",
            Help = "kleenestar.core:field.unique.help"
        };

        /// <summary>
        /// Gets the checkbox control for the deprecated flag.
        /// </summary>
        public ControlFormItemInputCheck FieldDeprecated { get; } = new()
        {
            Name = nameof(Model.Entities.Field.Deprecated),
            Label = "kleenestar.core:field.deprecated.label",
            Help = "kleenestar.core:field.deprecated.help"
        };

        /// <summary>
        /// Gets the input selection control for the access modifier.
        /// </summary>
        public ControlRestFormItemInputSelection AccessModifierSelection { get; } = new()
        {
            Name = nameof(Model.Entities.Field.AccessModifier),
            Label = "kleenestar.core:field.accessmodifier.label",
            Placeholder = "kleenestar.core:field.accessmodifier.placeholder",
            Help = "kleenestar.core:field.accessmodifier.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.AccessModifier>()
        };

        /// <summary>
        /// Gets the input text control for specifying the default specification.
        /// </summary>
        public ControlFormItemInputText DefaultSpec { get; } = new()
        {
            Name = nameof(Model.Entities.Field.DefaultSpec),
            Label = "kleenestar.core:field.defaultspec.label",
            Placeholder = "kleenestar.core:field.defaultspec.placeholder",
            Help = "kleenestar.core:field.defaultspec.help",
            Required = false
        };

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlRestFormItemInputSelection FieldState { get; } = new()
        {
            Name = nameof(Model.Entities.Field.State),
            Label = "kleenestar.core:field.state.label",
            Placeholder = "kleenestar.core:field.state.placeholder",
            Help = "kleenestar.core:field.state.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.State>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public FieldEditFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(FieldName);
            Add(Description);
            Add(HelpText);
            Add(FieldPlaceholder);
            Add(FieldTypeSelection);
            Add(CardinalitySelection);
            Add(FieldRequired);
            Add(FieldUnique);
            Add(FieldDeprecated);
            Add(AccessModifierSelection);
            Add(DefaultSpec);
            Add(FieldState);

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
