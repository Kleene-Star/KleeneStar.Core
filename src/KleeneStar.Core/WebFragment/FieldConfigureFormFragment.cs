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
    /// Represents the configure form fragment for a field.
    /// Provides tabbed controls for Cardinality, Validation, Options, Filter objects,
    /// Workflow, and Priority configuration categories.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Field._fieldid_.Configure>]
    [Cache]
    public sealed class FieldConfigureFormFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Gets the numeric input control for the minimum number of values the field must contain.
        /// </summary>
        public ControlFormItemInputText CardinalityMin { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Field.CardinalityMin),
            Label = _ => "kleenestar.core:field.configure.cardinality.min.label",
            Placeholder = _ => "kleenestar.core:field.configure.cardinality.min.placeholder",
            Help = _ => "kleenestar.core:field.configure.cardinality.min.help"
        };

        /// <summary>
        /// Gets the checkbox control that, when enabled, removes the upper bound on the field's value count.
        /// </summary>
        public ControlFormItemInputCheck CardinalityUnlimited { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Field.CardinalityUnlimited),
            Label = _ => "kleenestar.core:field.configure.cardinality.unlimited.label",
            Help = _ => "kleenestar.core:field.configure.cardinality.unlimited.help",
            Layout = TypeLayoutCheck.Switch
        };

        /// <summary>
        /// Gets the numeric input control for the maximum number of values the field may contain.
        /// Ignored when <see cref="CardinalityUnlimited"/> is enabled.
        /// </summary>
        public ControlFormItemInputText CardinalityMax { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Field.CardinalityMax),
            Label = _ => "kleenestar.core:field.configure.cardinality.max.label",
            Placeholder = _ => "kleenestar.core:field.configure.cardinality.max.placeholder",
            Help = _ => "kleenestar.core:field.configure.cardinality.max.help"
        };

        /// <summary>
        /// Gets the text input control for specifying a regular expression pattern to validate field values.
        /// Applies to text and string field types.
        /// </summary>
        public ControlFormItemInputText RegexPattern { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Field.RegexPattern),
            Label = _ => "kleenestar.core:field.configure.validation.regex.label",
            Placeholder = _ => "kleenestar.core:field.configure.validation.regex.placeholder",
            Help = _ => "kleenestar.core:field.configure.validation.regex.help",
            Required = _ => false
        };

        /// <summary>
        /// Gets the tag input control for maintaining the list of selectable option values
        /// for enumerable field types such as Selection.
        /// </summary>
        public ControlFormItemInputTag Options { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Field.Options),
            Label = _ => "kleenestar.core:field.configure.options.label",
            Placeholder = "kleenestar.core:field.configure.options.placeholder",
            Help = _ => "kleenestar.core:field.configure.options.help"
        };

        /// <summary>
        /// Gets the text input control for specifying a WQL filter expression that restricts
        /// permissible target objects for referential field types.
        /// </summary>
        public ControlFormItemInputText FilterWql { get; } = new()
        {
            Name = _ => "Wql",
            Label = _ => "kleenestar.core:field.configure.wql.label",
            Placeholder = _ => "kleenestar.core:field.configure.wql.placeholder",
            Help = _ => "kleenestar.core:field.configure.wql.help",
            Required = _ => false
        };

        /// <summary>
        /// Gets the selection control for assigning an active workflow to this field.
        /// Only active workflows compatible with the field's class are available.
        /// Applies to fields of type Workflow.
        /// </summary>
        public ControlRestFormItemInputSelection WorkflowSelection { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Field.WorkflowId),
            Label = _ => "kleenestar.core:field.configure.workflow.label",
            Placeholder = _ => "kleenestar.core:field.configure.workflow.placeholder",
            Help = _ => "kleenestar.core:field.configure.workflow.help",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workflows.Index>()
        };

        /// <summary>
        /// Gets the selection control for choosing the default priority for this field.
        /// Applies to fields of type Priority (Reference with priority semantics).
        /// </summary>
        public ControlRestFormItemInputSelection DefaultPrioritySelection { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Field.DefaultPriorityId),
            Label = _ => "kleenestar.core:field.configure.priority.default.label",
            Placeholder = _ => "kleenestar.core:field.configure.priority.default.placeholder",
            Help = _ => "kleenestar.core:field.configure.priority.default.help",
            StickySelection = _ => true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities.Index>()
        };

        /// <summary>
        /// Gets the dual-list transfer control for selecting the priorities available for this field.
        /// </summary>
        public ControlRestFormItemInputSelection SelectedPriorities { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Field.SelectedPriorityIds),
            Label = _ => "kleenestar.core:field.configure.priority.selected.label",
            Placeholder = _ => "kleenestar.core:field.configure.priority.available.label",
            Help = _ => "kleenestar.core:field.configure.priority.selected.label",
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Priorities.Index>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public FieldConfigureFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Mode = _ => TypeRestFormMode.Edit;
            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Fields.Index>();

            CardinalityMax.Bind = _ => new Binding().Add(new BindDisable()
            {
                Source = CardinalityUnlimited.Id,
                Condition = "true"
            });
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

            var tab = new ControlFormItemGroupTab()
            {
            }
                .AddView
                (
                    (IControlFormItemGroupTabView)new ControlFormItemGroupTabView()
                    {
                        Title = _ => "kleenestar.core:field.configure.tab.cardinality"
                    }
                        .Add(CardinalityMin)
                        .Add(CardinalityUnlimited)
                        .Add(CardinalityMax))
                .AddView
                (
                    (IControlFormItemGroupTabView)new ControlFormItemGroupTabView()
                    {
                        Title = _ => "kleenestar.core:field.configure.tab.validation"
                    }
                        .Add(RegexPattern)
                )
                .AddView
                (
                    (IControlFormItemGroupTabView)new ControlFormItemGroupTabView()
                    {
                        Title = _ => "kleenestar.core:field.configure.tab.options"
                    }
                        .Add(Options)
                )
                .AddView
                (
                    (IControlFormItemGroupTabView)new ControlFormItemGroupTabView()
                    {
                        Title = _ => "kleenestar.core:field.configure.tab.filter"
                    }
                        .Add(FilterWql)
                )
                .AddView
                (
                    (IControlFormItemGroupTabView)new ControlFormItemGroupTabView()
                    {
                        Title = _ => "kleenestar.core:field.configure.tab.workflow"
                    }
                        .Add(WorkflowSelection)
                )
                .AddView
                (
                    (IControlFormItemGroupTabView)new ControlFormItemGroupTabView()
                    {
                        Title = _ => "kleenestar.core:field.configure.tab.priority"
                    }
                        .Add(DefaultPrioritySelection)
                        .Add(SelectedPriorities)
                );

            return base.Render(renderContext, visualTree);
        }
    }
}

