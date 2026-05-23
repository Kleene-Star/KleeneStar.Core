using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Sla
{
    /// <summary>
    /// Clone-form fragment for an existing SLA policy. The new copy starts as a Draft.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Sla._slaid_.Clone>]
    [Cache]
    public sealed class SlaCloneFormFragment : FragmentControlRestFormClone
    {
        /// <summary>
        /// Gets the input control for the policy name.
        /// </summary>
        public ControlRestFormItemInputUnique SlaName { get; } = new()
        {
            Name = _ => nameof(Model.Entities.SlaPolicy.Name),
            Label = _ => "kleenestar.core:sla.name.label",
            Placeholder = _ => "kleenestar.core:sla.name.placeholder",
            Help = _ => "kleenestar.core:sla.name.help",
            Required = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Slas.UniqueName>()
        };

        /// <summary>
        /// Gets the textarea control for the description (WYSIWYG).
        /// </summary>
        public ControlFormItemInputText Description { get; } = new()
        {
            Name = _ => nameof(Model.Entities.SlaPolicy.Description),
            Label = _ => "kleenestar.core:sla.description.label",
            Placeholder = _ => "kleenestar.core:sla.description.placeholder",
            Format = _ => TypeEditTextFormat.Wysiwyg,
            Required = _ => false
        };

        /// <summary>
        /// Gets the selection control for the lifecycle state.
        /// </summary>
        public ControlRestFormItemInputSelection SlaState { get; } = new()
        {
            Name = _ => nameof(Model.Entities.SlaPolicy.State),
            Label = _ => "kleenestar.core:sla.state.label",
            Placeholder = _ => "kleenestar.core:sla.state.placeholder",
            Help = _ => "kleenestar.core:sla.state.help",
            StickySelection = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Slas.State>()
        };

        /// <summary>
        /// Gets the selection control for the severity priority.
        /// </summary>
        public ControlRestFormItemInputSelection SlaPriority { get; } = new()
        {
            Name = _ => nameof(Model.Entities.SlaPolicy.Priority),
            Label = _ => "kleenestar.core:sla.priority.label",
            Placeholder = _ => "kleenestar.core:sla.priority.placeholder",
            Help = _ => "kleenestar.core:sla.priority.help",
            StickySelection = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Slas.Priority>()
        };

        /// <summary>
        /// Gets the selection control for the working calendar.
        /// </summary>
        public ControlRestFormItemInputSelection SlaCalendar { get; } = new()
        {
            Name = _ => nameof(Model.Entities.SlaPolicy.Calendar),
            Label = _ => "kleenestar.core:sla.calendar.label",
            Placeholder = _ => "kleenestar.core:sla.calendar.placeholder",
            Help = _ => "kleenestar.core:sla.calendar.help",
            StickySelection = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Slas.Calendar>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        public SlaCloneFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(SlaName);
            Add(Description);
            Add(SlaState);
            Add(SlaPriority);
            Add(SlaCalendar);

            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Slas.Index>();
            ItemId = renderContext =>
            {
                var slaId = renderContext.Request.GetParameter<SlaIdParameter>();
                return slaId?.Value;
            };
        }

        /// <summary>
        /// Renders the form control as HTML.
        /// </summary>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
