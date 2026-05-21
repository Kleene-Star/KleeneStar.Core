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
    /// Personal access tokens — tokens used for API access and integrations.
    /// </summary>
    [Title("kleenestar.core:profile.tokens.title")]
    [WebIcon<IconKey>]
    [Scope<IScopeGeneral>]
    public sealed class Tokens : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Tokens()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            visualTree.Content.MainPanel.Headline.Title = I18N.Translate(renderContext, "kleenestar.core:profile.tokens.title");

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.tokens.header"),
                TextColor = _ => new PropertyColorText(TypeColorText.Info),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.tokens.description"),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            var table = new ControlTable()
            {
                Striped = _ => TypeStripedTable.Row
            }
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.tokens.column.name"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.tokens.column.prefix"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.tokens.column.scopes"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.tokens.column.expires"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.tokens.column.status"));

            visualTree.Content.MainPanel.AddPrimary(table);
        }
    }
}
