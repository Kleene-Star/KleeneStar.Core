using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Dashboard
{
    /// <summary>
    /// Represents an add form fragment for a dashboard.
    /// </summary>
    [Title("kleenestar.core:dashboard.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Dashboards.Add>]
    [Cache]
    public sealed class DashboardAddFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Gets the input text control for specifying the name of the dashboard.
        /// </summary>
        public ControlRestFormItemInputUnique DashboardName { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Dashboard.Name),
            Label = _ => "kleenestar.core:dashboard.name.label",
            Placeholder = _ => "kleenestar.core:dashboard.name.placeholder",
            Help = _ => "kleenestar.core:dashboard.name.help",
            Required = _ => true,
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Dashboards.UniqueName>()
        };

        /// <summary>
        /// Gets the input tag definition for the category field.
        /// </summary>
        public ControlFormItemInputTag Category { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Dashboard.Categories),
            Label = _ => "kleenestar.core:dashboard.category.label",
            Placeholder = _ => "kleenestar.core:dashboard.category.placeholder",
            Help = _ => "kleenestar.core:dashboard.category.help"
        };

        /// <summary>
        /// Gets the input text control for specifying the description of the dashboard.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = _ => nameof(Model.Entities.Dashboard.Description),
            Label = _ => "kleenestar.core:dashboard.description.label",
            Placeholder = _ => "kleenestar.core:dashboard.description.placeholder",
            Format = _ => TypeEditTextFormat.Wysiwyg,
            Required = _ => false
        };

        /// <summary>
        /// Gets the input selection control for the state.
        /// </summary>
        public ControlRestFormItemInputSelection DashboardState { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Dashboard.State),
            Label = _ => "kleenestar.core:dashboard.state.label",
            Placeholder = _ => "kleenestar.core:dashboard.state.placeholder",
            Help = _ => "kleenestar.core:dashboard.state.help",
            RestUri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Dashboards.State>()
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public DashboardAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(DashboardName);
            Add(Category);
            Add(Description);
            Add(DashboardState);

            Mode = _ => TypeRestFormMode.Add;
            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Dashboards.Index>();
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
            return base.Render(renderContext, visualTree);
        }
    }
}
