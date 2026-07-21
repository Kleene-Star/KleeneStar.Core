using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System.Linq;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    // The entity type names collide with the KleeneStar.Core.WWW.* namespace segments of
    // the same name; alias them inside the namespace block so Field/Status/Workflow resolve
    // to the model entities here (see also the Calendar namespace-collision note).
    using Field = KleeneStar.Model.Entities.Field;
    using Status = KleeneStar.Model.Entities.Status;
    using Workflow = KleeneStar.Model.Entities.Workflow;

    /// <summary>
    /// Object-scoped property card that renders, for every workflow-backed field of the
    /// current object's class, a split button reflecting the field's current status on
    /// <see cref="WWW.Issue._objectkey_.Index"/>.
    /// </summary>
    /// <remarks>
    /// A field qualifies when it is active, not deprecated, of type
    /// <see cref="FieldType.Workflow"/>, and carries a <see cref="Field.WorkflowId"/>.
    /// For each such field the card hosts a <see cref="ControlSplitButton"/> whose main
    /// button shows the field's current status (resolved from the persisted
    /// <see cref="Model.Entities.Value"/> against the workflow's statuses), whose dropdown
    /// lists every status of the attached workflow, and whose final item opens the
    /// workflow itself in a modal (<see cref="WWW.Workflow._workflowid_.Index"/>). These
    /// fields are intentionally omitted from the form-driven detail view rendered by
    /// <see cref="ObjectItemDetailFragment"/> so the status lives only in this card.
    /// </remarks>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Asset._objectkey_.Index>]
    [Order(1)]
    [Cache]
    public sealed class ObjectPropertyWorkflowCardFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;
        private readonly IFieldManager _fieldManager;
        private readonly IWorkflowManager _workflowManager;
        private readonly IValueManager _valueManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the current
        /// object from the URL-bound object key.</param>
        /// <param name="fieldManager">The field manager used to enumerate the class fields.</param>
        /// <param name="workflowManager">The workflow manager used to load the workflow
        /// attached to a workflow-type field.</param>
        /// <param name="valueManager">The value manager used to read the object's current
        /// field values.</param>
        public ObjectPropertyWorkflowCardFragment
        (
            IFragmentContext fragmentContext,
            IObjectManager objectManager,
            IFieldManager fieldManager,
            IWorkflowManager workflowManager,
            IValueManager valueManager
        )
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _fieldManager = fieldManager;
            _workflowManager = workflowManager;
            _valueManager = valueManager;
        }

        /// <summary>
        /// Renders the workflow status card for the current object. Returns <c>null</c>
        /// when the fragment's render conditions exclude it, when no object can be resolved
        /// from the request, or when the object's class has no workflow-backed fields.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The HTML node, or <c>null</c>.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var keyParameter = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            var @object = _objectManager.GetObjectByKey(keyParameter?.Value);

            if (@object is null)
            {
                return null;
            }

            var workflowFields = _fieldManager
                .GetFields(new ClassIdParameter(@object.ClassId))
                .Where(f => !f.Deprecated
                    && f.State == FieldState.Active
                    && f.FieldType == FieldType.Workflow
                    && f.WorkflowId.HasValue)
                .ToList();

            if (workflowFields.Count == 0)
            {
                return null;
            }

            var card = new ControlPanelCard("object-property-workflow-card")
            {
                Header = _ => "kleenestar.core:object.property.workflow.header",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two)
            };

            var rendered = false;

            foreach (var field in workflowFields)
            {
                var block = BuildFieldBlock(@object, field);
                if (block is not null)
                {
                    card.Add(block);
                    rendered = true;
                }
            }

            return rendered ? card.Render(renderContext, visualTree) : null;
        }

        /// <summary>
        /// Builds the labelled split-button block for a single workflow-backed field, or
        /// <c>null</c> when the attached workflow can no longer be resolved.
        /// </summary>
        /// <param name="object">The object whose status is displayed.</param>
        /// <param name="field">The workflow-type field being rendered.</param>
        /// <returns>The control hosting the field label and split button, or <c>null</c>.</returns>
        private IControl BuildFieldBlock(Model.Entities.Object @object, Field field)
        {
            var workflow = _workflowManager.GetWorkflow(field.WorkflowId.Value);

            if (workflow is null)
            {
                return null;
            }

            var value = _valueManager.GetValue(@object.Id, field.Id);
            var current = ResolveStatus(workflow, value?.Data);
            var currentLabel = current?.Name
                ?? (string.IsNullOrWhiteSpace(value?.Data) ? null : value.Data);

            var split = new ControlSplitButton("object-workflow-" + field.Id.ToString("N"))
            {
                Text = ctx => currentLabel
                    ?? WebExpress.WebCore.Internationalization.I18N.Translate(ctx, "kleenestar.core:object.property.workflow.notset.label"),
                Icon = _ => new IconStatus(TypeIconTheme.Light),
                BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Primary)
            };

            foreach (var status in workflow.Statuses ?? [])
            {
                split.Add(new ControlSplitButtonItemLink("object-workflow-status-" + status.Id.ToString("N"))
                {
                    Text = _ => status.Name,
                    Icon = _ => new IconStatus(TypeIconTheme.Light),
                    Uri = ctx => CoreHub.GetUri<global::KleeneStar.Core.WWW.Status._statusid_.Index>()?
                        .BindParameters(new WorkflowStateIdParameter(status.Id))
                        .BindParameters(ctx.Request)
                });
            }

            split.AddDivider();

            split.Add(new ControlSplitButtonItemLink("object-workflow-show-" + field.Id.ToString("N"))
            {
                Text = _ => "kleenestar.core:object.property.workflow.show.label",
                Icon = _ => new IconWorkflow(TypeIconTheme.Light),
                PrimaryAction = ctx => new ActionModal
                (
                    "modal-form",
                    CoreHub.GetUri<global::KleeneStar.Core.WWW.Workflow._workflowid_.Index>()?
                        .BindParameters(new WorkflowIdParameter(workflow.Id))
                        .BindParameters(ctx.Request),
                    TypeModalSize.ExtraLarge
                )
            });

            var panel = new ControlPanel("object-workflow-field-" + field.Id.ToString("N"))
            {
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Two)
            };

            panel.Add(new ControlText("object-workflow-label-" + field.Id.ToString("N"))
            {
                Text = _ => field.Name,
                Format = _ => TypeFormatText.Small
            });
            panel.Add(split);

            return panel;
        }

        /// <summary>
        /// Resolves the persisted field value to a <see cref="Status"/> of the supplied
        /// workflow. The match is attempted first by normalised name (case-, space- and
        /// punctuation-insensitive, so <c>in_progress</c> matches <c>In Progress</c>) and
        /// then by status id. Returns <c>null</c> when the value is empty or no status
        /// matches, in which case the raw value is shown verbatim.
        /// </summary>
        /// <param name="workflow">The workflow whose statuses are searched.</param>
        /// <param name="data">The persisted value payload of the workflow field.</param>
        /// <returns>The matching status, or <c>null</c>.</returns>
        private static Status ResolveStatus(Workflow workflow, string data)
        {
            if (string.IsNullOrWhiteSpace(data) || workflow.Statuses is null)
            {
                return null;
            }

            var normalized = Normalize(data);

            return workflow.Statuses.FirstOrDefault(s => Normalize(s.Name) == normalized)
                ?? workflow.Statuses.FirstOrDefault(s => string.Equals(s.Id.ToString(), data, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Reduces a string to its lower-cased alphanumeric characters so loosely-formatted
        /// status slugs can be compared against status names.
        /// </summary>
        /// <param name="value">The value to normalise.</param>
        /// <returns>The normalised string.</returns>
        private static string Normalize(string value)
        {
            return new string((value ?? string.Empty).Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        }
    }
}
