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
    /// Active sessions — devices and browsers currently signed in with this account.
    /// </summary>
    [Title("kleenestar.core:profile.sessions.title")]
    [WebIcon<IconLaptop>]
    [Scope<IScopeGeneral>]
    public sealed class Sessions : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Sessions()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            visualTree.Content.MainPanel.Headline.Title = I18N.Translate(renderContext, "kleenestar.core:profile.sessions.title");

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.sessions.header"),
                TextColor = _ => new PropertyColorText(TypeColorText.Info),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => I18N.Translate(renderContext, "kleenestar.core:profile.sessions.description"),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            var table = new ControlTable()
            {
                Striped = _ => TypeStripedTable.Row
            }
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.sessions.column.device"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.sessions.column.browser"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.sessions.column.location"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:profile.sessions.column.lastactive"));

            visualTree.Content.MainPanel.AddPrimary(table);
        }
    }
}
