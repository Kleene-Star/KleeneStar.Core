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
    /// Represents an edit form fragment for a group.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Settings.Group._groupid_.Edit>]
    [Cache]
    public sealed class GroupEditFormFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Gets the input for the group name.
        /// </summary>
        public ControlRestFormItemInputUnique GroupName { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Group.Name),
            Label = _ => "kleenestar.core:setting.group.name.label",
            Placeholder = _ => "kleenestar.core:setting.group.name.placeholder",
            Help = _ => "kleenestar.core:setting.group.name.help",
            Required = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Groups.UniqueName>()
        };

        /// <summary>
        /// Gets the input for the description.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = _ => nameof(Model.Entities.Group.Description),
            Label = _ => "kleenestar.core:setting.group.description.label",
            Placeholder = _ => "kleenestar.core:setting.group.description.placeholder",
            Format = _ => TypeEditTextFormat.Wysiwyg,
            Required = _ => false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GroupEditFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(GroupName);
            Add(Description);

            Mode = _ => TypeRestFormMode.Edit;
            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Groups.Index>();
        }

        /// <summary>
        /// Renders the control as HTML.
        /// </summary>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            var param = renderContext.Request.GetParameter<GroupIdParameter>();

            return base.Render(renderContext, visualTree);
        }
    }
}
