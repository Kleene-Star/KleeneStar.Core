using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Quickfilters
{
    /// <summary>
    /// Represents the dialog in which a user defines a new quickfilter for the bar it was opened
    /// from.
    /// </summary>
    /// <remarks>
    /// The page carries no content of its own; the form is contributed by
    /// <see cref="WebFragment.Quickfilter.QuickfilterAddFormFragment"/>, following the way the other
    /// add dialogs are composed. Which bar the filter is destined for travels in the <c>view</c> and
    /// <c>context</c> query parameters the chip puts on this address.
    /// </remarks>
    [WebIcon<IconFilter>]
    [Title("kleenestar.core:quickfilter.add.title")]
    [Scope<IScopeGeneral>]
    public sealed class Add : IPage<VisualTreeWebApp>, IScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Add()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
        }
    }
}
