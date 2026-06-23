using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.SavedSearch._savedsearchid_
{
    /// <summary>
    /// Canonical route for a single saved search. Its main purpose is to declare the
    /// <c>_savedsearchid_</c> path segment so the edit and delete sibling pages can bind
    /// the <see cref="SavedSearchIdParameter"/>.
    /// </summary>
    [WebIcon<IconMagnifyingGlass>]
    [SavedSearchIdSegment]
    [Scope<IScopeGeneral>]
    [Domain<Model.Entities.SavedSearch>]
    [Cache]
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
            var parameter = renderContext.Request.GetParameter<SavedSearchIdParameter>();
            var savedSearch = CoreHub.SavedSearchManager.GetSavedSearch(parameter);

            visualTree.Title = savedSearch?.Name;
            visualTree.Content.MainPanel.Headline.Title = savedSearch?.Name;
        }
    }
}
