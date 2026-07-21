using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebControl
{
    /// <summary>
    /// Represents the global search dropdown for the application header. Its built-in
    /// search box queries all objects (regardless of kind) and its dynamic items are the
    /// calling identity's most recently opened objects; a titled static entry below opens
    /// the global search page. This is the dedicated search entry that was split out of
    /// the former single object dropdown when it was replaced by the per-kind dropdowns.
    /// </summary>
    public class SearchDropdownControl : ControlDataDropdown
    {
        /// <summary>
        /// Gets the static link that opens the global search page (search over all
        /// workspaces).
        /// </summary>
        public ControlDropdownItemLink Search { get; } = new()
        {
            Text = _ => "kleenestar.core:search.dropdown.all.label",
            Icon = _ => new IconMagnifyingGlass(TypeIconTheme.Light),
            Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Search.Index>(),
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the dropdown control.</param>
        public SearchDropdownControl(string id)
            : base(id)
        {
            Text = _ => "kleenestar.core:search.dropdown.label";
            Icon = _ => new IconMagnifyingGlass(TypeIconTheme.Light);
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects.Dropdown>().ToString());

            // the recent objects (dynamic) render at the top; the framework inserts a
            // divider before the static items below. Title that section and add the
            // global-search entry.
            AddHeader("kleenestar.core:object.dropdown.search.label");
            Add(Search);
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
