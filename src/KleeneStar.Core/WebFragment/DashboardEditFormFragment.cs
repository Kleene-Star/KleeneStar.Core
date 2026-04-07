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
    /// Represents an edit form fragment for a dashboard.
    /// </summary>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Dashboard._dashboardid_.Edit>]
    [Cache]
    public sealed class DashboardEditFormFragment : FragmentControlRestFormEdit
    {
        /// <summary>
        /// Returns the input text control for specifying the name of the dashboard.
        /// </summary>
        public ControlRestFormItemInputUnique DashboardName { get; } = new()
        {
            Name = nameof(Model.Entities.Dashboard.Name),
            Label = "kleenestar.core:dashboard.name.label",
            Placeholder = "kleenestar.core:dashboard.name.placeholder",
            Help = "kleenestar.core:dashboard.name.help",
            Required = true,
            RestUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Dashboards.UniqueName>()
        };

        /// <summary>
        /// Returns the input text control for specifying the description of the dashboard.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Model.Entities.Dashboard.Description),
            Label = "kleenestar.core:dashboard.description.label",
            Placeholder = "kleenestar.core:dashboard.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public DashboardEditFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(DashboardName);
            Add(Description);

            Mode = TypeRestFormMode.Edit;
            Uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Dashboards.Index>();
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
            var param = renderContext.Request.GetParameter<DashboardIdParameter>();

            return base.Render(renderContext, visualTree, Items, param?.Value, Uri);
        }
    }
}
