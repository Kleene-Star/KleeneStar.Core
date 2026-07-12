using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Profile
{
    /// <summary>
    /// Profile landing page — publicly visible information that other members of the
    /// active tenant can see (display name, avatar, contact channels).
    /// </summary>
    [Title("kleenestar.core:profile.title")]
    [WebIcon<IconCircleUser>]
    [Scope<IScopeGeneral>]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            visualTree.Content.MainPanel.Headline.Title = I18N.Translate(renderContext, "kleenestar.core:profile.title");

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.header"),
                TextColor = _ => new PropertyColorText(TypeColorText.Info),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.description"),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            var table = new ControlTable()
            {
                Striped = _ => TypeStripedTable.Row,
                SuppressHeaders = _ => true
            }
                .AddColumn("")
                .AddColumn("");

            AddRow(table, renderContext, "kleenestar.core:profile.field.displayname.label", "kleenestar.core:profile.field.displayname.placeholder");
            AddRow(table, renderContext, "kleenestar.core:profile.field.bio.label", "kleenestar.core:profile.field.bio.placeholder");
            AddRow(table, renderContext, "kleenestar.core:profile.field.phone.label", "kleenestar.core:profile.field.phone.placeholder");
            AddRow(table, renderContext, "kleenestar.core:profile.field.website.label", "kleenestar.core:profile.field.website.placeholder");
            AddRow(table, renderContext, "kleenestar.core:profile.field.location.label", "kleenestar.core:profile.field.location.placeholder");
            AddRow(table, renderContext, "kleenestar.core:profile.field.position.label", "kleenestar.core:profile.field.position.placeholder");

            visualTree.Content.MainPanel.AddPrimary(table);
        }

        /// <summary>
        /// Adds a row with a translated label and value to the control table.
        /// </summary>
        /// <param name="table">
        /// The control table to add the row to.
        /// </param>
        /// <param name="renderContext">
        /// The render context used for translating the label and value.
        /// </param>
        /// <param name="labelKey">
        /// The translation key for the label cell.
        /// </param>
        /// <param name="valueKey">
        /// The translation key for the value cell.
        /// </param>
        private static void AddRow(IControlTable table, IRenderContext renderContext, string labelKey, string valueKey)
        {
            table.AddRow
            (
                new ControlTableCell() { Text = _ => I18N.Translate(renderContext, labelKey) },
                new ControlTableCellPanel().Add(new ControlText() { Text = _ => I18N.Translate(renderContext, valueKey) })
            );
        }
    }
}
