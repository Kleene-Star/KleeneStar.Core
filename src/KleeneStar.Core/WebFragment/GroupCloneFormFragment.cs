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
    /// Represents a clone form fragment for a group.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Settings.Group._groupid_.Clone>]
    [Cache]
    public sealed class GroupCloneFormFragment : FragmentControlRestFormClone
    {
        /// <summary>
        /// Gets the input for the group name.
        /// </summary>
        public ControlRestFormItemInputUnique GroupName { get; } = new()
        {
            Name = nameof(Model.Entities.Group.Name),
            Label = "kleenestar.core:setting.group.name.label",
            Placeholder = "kleenestar.core:setting.group.name.placeholder",
            Help = "kleenestar.core:setting.group.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Groups.UniqueName>()
        };

        /// <summary>
        /// Gets the input for the description.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.Group.Description),
            Label = "kleenestar.core:setting.group.description.label",
            Placeholder = "kleenestar.core:setting.group.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GroupCloneFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(GroupName);
            Add(Description);

            Mode = TypeRestFormMode.Clone;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Groups.Index>();
        }

        /// <summary>
        /// Renders the control as HTML.
        /// </summary>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            var param = renderContext.Request.GetParameter<GroupIdParameter>();

            return base.Render(renderContext, visualTree, Items, param?.Value, Uri);
        }
    }
}
