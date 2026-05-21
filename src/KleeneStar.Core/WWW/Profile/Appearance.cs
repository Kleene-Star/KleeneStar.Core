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
    /// Appearance preferences — theme, accent color, density, font scaling.
    /// </summary>
    [Title("kleenestar.core:profile.appearance.title")]
    [WebIcon<IconPalette>]
    [Scope<IScopeGeneral>]
    public sealed class Appearance : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Appearance()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            visualTree.Content.MainPanel.Headline.Title = I18N.Translate(renderContext, "kleenestar.core:profile.appearance.title");

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.appearance.header"),
                TextColor = _ => new PropertyColorText(TypeColorText.Info),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.appearance.description"),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            var table = new ControlTable()
            {
                Striped = _ => TypeStripedTable.Row,
                SuppressHeaders = _ => true
            }
                .AddColumn("")
                .AddColumn("");

            AddRow(table, renderContext, "kleenestar.core:profile.appearance.theme.label", "kleenestar.core:profile.appearance.theme.help");
            AddRow(table, renderContext, "kleenestar.core:profile.appearance.accent.label", "kleenestar.core:profile.appearance.accent.help");
            AddRow(table, renderContext, "kleenestar.core:profile.appearance.density.label", "kleenestar.core:profile.appearance.density.help");
            AddRow(table, renderContext, "kleenestar.core:profile.appearance.fontscale.label", "kleenestar.core:profile.appearance.fontscale.help");
            AddRow(table, renderContext, "kleenestar.core:profile.appearance.reducemotion.label", "kleenestar.core:profile.appearance.reducemotion.help");

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
