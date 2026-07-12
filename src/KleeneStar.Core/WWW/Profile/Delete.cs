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
    /// Account deactivation and deletion. Irreversible actions are guarded by description text.
    /// </summary>
    [Title("kleenestar.core:profile.delete.title")]
    [WebIcon<IconTrashCan>]
    [Scope<IScopeGeneral>]
    public sealed class Delete : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Delete()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            visualTree.Content.MainPanel.Headline.Title = I18N.Translate(renderContext, "kleenestar.core:profile.delete.title");

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.delete.header"),
                TextColor = _ => new PropertyColorText(TypeColorText.Danger),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.delete.description"),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            var table = new ControlTable()
            {
                Striped = _ => TypeStripedTable.Row,
                SuppressHeaders = _ => true
            }
                .AddColumn("")
                .AddColumn("");

            AddRow(table, renderContext, "kleenestar.core:profile.delete.deactivate.label", "kleenestar.core:profile.delete.deactivate.help");
            AddRow(table, renderContext, "kleenestar.core:profile.delete.remove.label", "kleenestar.core:profile.delete.remove.help");

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
