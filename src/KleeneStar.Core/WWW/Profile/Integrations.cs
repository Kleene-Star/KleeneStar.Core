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
    /// Per-user integrations — accounts and services connected to this user.
    /// </summary>
    [Title("kleenestar.core:profile.integrations.title")]
    [WebIcon<IconPlug>]
    [Scope<IScopeGeneral>]
    public sealed class Integrations : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Integrations()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            visualTree.Content.MainPanel.Headline.Title = I18N.Translate(renderContext, "kleenestar.core:profile.integrations.title");

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.integrations.header"),
                TextColor = _ => new PropertyColorText(TypeColorText.Info),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.integrations.description"),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            var table = new ControlTable()
            {
                Striped = _ => TypeStripedTable.Row
            }
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.integrations.column.service"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.integrations.column.description"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.integrations.column.status"));

            visualTree.Content.MainPanel.AddPrimary(table);
        }
    }
}
