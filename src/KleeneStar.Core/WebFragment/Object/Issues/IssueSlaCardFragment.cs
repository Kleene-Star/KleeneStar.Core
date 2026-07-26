using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System.Linq;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object.Issues
{
    /// <summary>
    /// Object-scoped fragment that renders a card displaying every SLA policy attached
    /// to the current object's class on <see cref="WWW.Issue._objectkey_.Index"/>.
    /// </summary>
    /// <remarks>
    /// The fragment resolves the object addressed by the request, looks up all active
    /// <see cref="SlaPolicy"/> entries on the object's class, and renders them inside
    /// a <see cref="ControlPanelCard"/>. Each policy shows its name, severity bucket,
    /// and configured targets (response/resolution/...) with their numeric value and
    /// unit. When no policies are configured for the class the card shows a localized
    /// empty-state message.
    /// </remarks>
    [Section<SectionPropertyPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>]
    [Order(0)]
    [Cache]
    public sealed class IssueSlaCardFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;
        private readonly ISlaManager _slaManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the current
        /// object from the URL-bound object key.</param>
        /// <param name="slaManager">The SLA manager used to load the policies attached
        /// to the resolved object's class.</param>
        public IssueSlaCardFragment(IFragmentContext fragmentContext, IObjectManager objectManager, ISlaManager slaManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _slaManager = slaManager;
        }

        /// <summary>
        /// Renders the SLA card for the current object.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>An HTML node, or <c>null</c> when the fragment's render conditions
        /// exclude it or when no object can be resolved from the request.</returns>
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

            var card = new ControlPanelCard("object-sla-card")
            {
                Header = _ => "kleenestar.core:object.sla.card.header",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two)
            };

            var policies = _slaManager
                .GetSlas(@object.ClassId)
                .Where(p => p.State == SlaPolicyState.Active)
                .OrderBy(p => p.Priority)
                .ThenBy(p => p.Name)
                .ToList();

            if (policies.Count == 0)
            {
                card.Add(new ControlText("object-sla-empty")
                {
                    Text = _ => "kleenestar.core:object.sla.card.none",
                    Format = _ => TypeFormatText.Italic
                });

                return card.Render(renderContext, visualTree);
            }

            foreach (var policy in policies)
            {
                card.Add(BuildPolicyBlock(policy));
            }

            return card.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Builds a panel containing the policy name, its severity bucket, and the
        /// configured targets formatted as "Kind: value unit".
        /// </summary>
        private static IControl BuildPolicyBlock(SlaPolicy policy)
        {
            var panel = new ControlPanel("object-sla-policy-" + policy.Id.ToString("N"));

            panel.Add(new ControlText
            {
                Text = _ => policy.Name,
                Format = _ => TypeFormatText.Strong
            });

            panel.Add(new ControlText
            {
                Text = _ => policy.Priority.TranslationKey(),
                Format = _ => TypeFormatText.Small
            });

            var targets = policy.Targets?.OrderBy(t => t.Kind).ToList() ?? [];
            if (targets.Count == 0)
            {
                return panel;
            }

            var list = new ControlList("object-sla-targets-" + policy.Id.ToString("N"));
            foreach (var target in targets)
            {
                list.Add(new ControlListItem
                {
                    Text = renderContext => FormatTarget(renderContext, target)
                });
            }

            panel.Add(list);

            return panel;
        }

        /// <summary>
        /// Returns the textual rendering of a target row, e.g. "First response: 30 Minutes".
        /// </summary>
        private static string FormatTarget(IRenderControlContext renderContext, SlaTarget target)
        {
            var culture = renderContext?.Request?.Culture;

            var kind = I18N.Translate(culture, target.Kind.TranslationKey());
            var unit = I18N.Translate(culture, target.Unit.TranslationKey());
            var name = string.IsNullOrWhiteSpace(target.Name) ? kind : target.Name;

            return $"{name}: {target.TargetValue} {unit}";
        }
    }

    /// <summary>
    /// Provides translation-key lookups for SLA enums used by the card layout.
    /// </summary>
    internal static class SlaTranslationKeyExtensions
    {
        /// <summary>
        /// Returns the i18n key for an SLA priority bucket.
        /// </summary>
        public static string TranslationKey(this SlaPriority priority)
        {
            return priority switch
            {
                SlaPriority.Low => "kleenestar.core:sla.priority.low.label",
                SlaPriority.Medium => "kleenestar.core:sla.priority.medium.label",
                SlaPriority.High => "kleenestar.core:sla.priority.high.label",
                SlaPriority.Critical => "kleenestar.core:sla.priority.critical.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the i18n key for an SLA target milestone.
        /// </summary>
        public static string TranslationKey(this SlaTargetKind kind)
        {
            return kind switch
            {
                SlaTargetKind.Response => "kleenestar.core:sla.targetkind.response.label",
                SlaTargetKind.Resolution => "kleenestar.core:sla.targetkind.resolution.label",
                SlaTargetKind.Update => "kleenestar.core:sla.targetkind.update.label",
                SlaTargetKind.Approval => "kleenestar.core:sla.targetkind.approval.label",
                SlaTargetKind.Implementation => "kleenestar.core:sla.targetkind.implementation.label",
                SlaTargetKind.Fulfillment => "kleenestar.core:sla.targetkind.fulfillment.label",
                SlaTargetKind.Custom => "kleenestar.core:sla.targetkind.custom.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the i18n key for an SLA target unit.
        /// </summary>
        public static string TranslationKey(this SlaTargetUnit unit)
        {
            return unit switch
            {
                SlaTargetUnit.Minutes => "kleenestar.core:sla.targetunit.minutes.label",
                SlaTargetUnit.Hours => "kleenestar.core:sla.targetunit.hours.label",
                SlaTargetUnit.Days => "kleenestar.core:sla.targetunit.days.label",
                SlaTargetUnit.BusinessDays => "kleenestar.core:sla.targetunit.businessdays.label",
                _ => null
            };
        }
    }
}
