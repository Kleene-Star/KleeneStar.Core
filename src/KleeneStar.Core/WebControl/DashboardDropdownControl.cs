using WebExpress.WebApp.WebApiControl;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebControl
{
    /// <summary>
    /// Represents a dropdown control for selecting a dashboard.
    /// </summary>
    public class DashboardDropdownControl : ControlRestDropdown
    {
        /// <summary>
        /// Gets the control link for adding a new dashboard.
        /// </summary>
        public ControlDropdownItemLink AddDashboard { get; } = new()
        {
            Text = "kleenestar.core:dashboard.add.label",
            Icon = new IconPlus(),
            PrimaryAction = new ActionModal("modal-form", CoreHub.GetUri<WWW.Dashboards.Add>(), TypeModalSize.ExtraLarge),
        };

        /// <summary>
        /// Gets the control link for managing dashboards.
        /// </summary>
        public ControlDropdownItemLink ManageDashboard { get; } = new()
        {
            Text = "kleenestar.core:dashboard.manage.label",
            Uri = CoreHub.GetUri<WWW.Dashboards.Index>(),
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the dropdown control.</param>
        public DashboardDropdownControl(string id)
            : base(id)
        {
            RestUri = CoreHub.GetUri<WWW.Api._1_.Dashboards.Dropdown>();

            Add(AddDashboard);
            Add(ManageDashboard);
        }

        /// <summary>
        /// Converts the control to an HTML representation.
        /// </summary>
        /// <param name="renderContext">The context in which the control is rendered.</param>
        /// <param name="visualTree">The visual tree representing the control's structure.</param>
        /// <returns>An HTML node representing the rendered control.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
