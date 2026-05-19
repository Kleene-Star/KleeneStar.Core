using WebExpress.WebApp.WebTheme;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIcon;

namespace KleeneStar.Core.WebTheme
{
    /// <summary>
    /// The default KleeneStar theme. Declares <c>[IconTheme(TypeIconTheme.Light)]</c>
    /// so the visual tree emits <c>&lt;html data-icon-theme="light"&gt;</c> and the
    /// JS / server-side icon resolution picks the lightweight SVG variants shipped
    /// via <c>webexpress.webui.icon.css</c>.
    /// </summary>
    [Name("kleenestar.core:theme.light.name")]
    [Description("kleenestar.core:theme.light.description")]
    [IconTheme(TypeIconTheme.Light)]
    public sealed class LightTheme : IThemeWebApp
    {
    }
}
