using KleeneStar.Core.WebParameter;
using System.Collections.Generic;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Search
{
    // The entity type SavedSearch collides with the sibling WebFragment.SavedSearch
    // namespace; alias it (inside the namespace block) so the bare name binds to the entity.
    using SavedSearch = KleeneStar.Model.Entities.SavedSearch;

    /// <summary>
    /// Renders the saved searches of the calling identity into the global search page
    /// sidebar: a "new search" entry, the saved searches (starred first), and a "new saved
    /// search" action. Each saved search runs on click and can be edited on double-click.
    /// </summary>
    [Section<SectionSidebarPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Search.Index>]
    [Cache]
    public sealed class SearchSidebarSavedFragment : FragmentControlSidebarItemLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public SearchSidebarSavedFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>
        /// Convert the fragment to HTML — the saved-search sidebar block.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node listing the saved-search sidebar items.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var ownerId = CoreHub.SessionManager.GetCurrentIdentityId(renderContext?.Request);
            var addUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.SavedSearches.Add>();

            var nodes = new List<IHtmlNode>
            {
                // a search entry-point that clears any applied query
                new ControlSidebarItemLink("search-all")
                {
                    Text = _ => "kleenestar.core:search.sidebar.new.label",
                    Uri = _ => CoreHub.GetUri<global::KleeneStar.Core.WWW.Search.Index>()
                }
                    .Render(renderContext, visualTree),

                new ControlSidebarItemHeader("saved-header")
                {
                    Text = _ => "kleenestar.core:search.sidebar.saved.heading"
                }
                    .Render(renderContext, visualTree)
            };

            foreach (var savedSearch in CoreHub.SavedSearchManager.GetForOwner(ownerId))
            {
                var captured = savedSearch;
                var editUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.SavedSearch._savedsearchid_.Edit>()?
                    .BindParameters(new SavedSearchIdParameter(captured.Id));

                nodes.Add(new ControlSidebarItemLink($"ss-{captured.Id}")
                {
                    Text = _ => (captured.Starred ? "★ " : string.Empty) + captured.Name,
                    Tooltip = _ => captured.Query,
                    Uri = _ => RunUri(captured),
                    SecondaryAction = _ => new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge)
                }
                    .Render(renderContext, visualTree));
            }

            nodes.Add(new ControlSidebarItemLink("saved-new")
            {
                Text = _ => "kleenestar.core:search.sidebar.add.label",
                PrimaryAction = _ => new ActionModal("modal-form", addUri, TypeModalSize.ExtraLarge)
            }
                .Render(renderContext, visualTree));

            return new HtmlList(nodes);
        }

        /// <summary>
        /// Builds the URI that runs the given saved search — the global search page with the
        /// saved query applied and the saved-search id flagged for recency tracking.
        /// </summary>
        /// <param name="savedSearch">The saved search to run.</param>
        /// <returns>The run URI.</returns>
        private static IUri RunUri(SavedSearch savedSearch)
        {
            return CoreHub.GetUri<global::KleeneStar.Core.WWW.Search.Index>()?
                .Add(new UriQuery("wql", savedSearch.Query ?? string.Empty))
                .Add(new UriQuery("use", savedSearch.Id.ToString()));
        }
    }
}
