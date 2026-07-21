using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System.Linq;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
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
    /// Headline-metadata fragment that surfaces the current workflow status of the object on
    /// <see cref="WWW.Issue._objectkey_.Index"/> as a read-only pill badge.
    /// </summary>
    /// <remarks>
    /// The status is resolved exactly like the interactive
    /// <see cref="ObjectPropertyWorkflowCardFragment"/> (the persisted
    /// <see cref="Model.Entities.Value"/> of every workflow-backed field matched against the
    /// attached workflow's statuses), but rendered as a plain badge without the split button,
    /// dropdown or workflow modal — so the status is displayed but cannot be changed from
    /// here. Returns <c>null</c> when the class has no workflow field or none has a value yet,
    /// keeping the metadata line clean.
    /// </remarks>
    [Section<SectionHeadlineMetadata>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Asset._objectkey_.Index>]
    [Order(0)]
    [Cache]
    public sealed class ObjectMetadataStatusFragment : FragmentControlPanel
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
        public ObjectMetadataStatusFragment
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
        /// Renders the read-only status badge(s) for the current object. Returns <c>null</c>
        /// when the fragment's render conditions exclude it, when no object can be resolved
        /// from the request, or when no workflow status can be determined.
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

            var panel = new ControlPanelFlex("object-metadata-status")
            {
                Layout = _ => TypeLayoutFlex.Default,
                Align = _ => TypeAlignFlex.Center,
                Justify = _ => TypeJustifiedFlex.Start
            };

            var rendered = false;

            foreach (var field in workflowFields)
            {
                var badge = BuildStatusBadge(@object, field);
                if (badge is not null)
                {
                    panel.Add(badge);
                    rendered = true;
                }
            }

            return rendered ? panel.Render(renderContext, visualTree) : null;
        }

        /// <summary>
        /// Builds the read-only status badge for a single workflow-backed field, or
        /// <c>null</c> when the workflow cannot be resolved or the field has no value yet.
        /// </summary>
        /// <param name="object">The object whose status is displayed.</param>
        /// <param name="field">The workflow-type field being rendered.</param>
        /// <returns>The badge control, or <c>null</c>.</returns>
        private IControl BuildStatusBadge(Model.Entities.Object @object, Field field)
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

            if (string.IsNullOrWhiteSpace(currentLabel))
            {
                return null;
            }

            return new ControlBadge("object-metadata-status-" + field.Id.ToString("N"))
            {
                Value = _ => currentLabel,
                Pill = _ => TypePillBadge.Pill,
                BackgroundColor = _ => new PropertyColorBackgroundBadge(TypeColorBackgroundBadge.Info),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.One, PropertySpacing.Space.None, PropertySpacing.Space.None)
            };
        }

        /// <summary>
        /// Resolves the persisted field value to a <see cref="Status"/> of the supplied
        /// workflow. The match is attempted first by normalised name (case-, space- and
        /// punctuation-insensitive) and then by status id. Returns <c>null</c> when the value
        /// is empty or no status matches, in which case the raw value is shown verbatim.
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
