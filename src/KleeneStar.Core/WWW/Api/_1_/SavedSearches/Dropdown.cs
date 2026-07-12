using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.SavedSearches
{
    // The entity type SavedSearch collides with the sibling WWW.SavedSearch namespace;
    // alias it (inside the namespace block) so the bare name binds to the entity.
    using SavedSearch = KleeneStar.Model.Entities.SavedSearch;

    /// <summary>
    /// Provides the navigation-dropdown items for the global search entry: the calling
    /// identity's ten most recently run saved searches, ordered most-recent first.
    /// </summary>
    [Title("kleenestar.core:search.dropdown.label")]
    [Cache]
    public sealed class Dropdown : RestApiDropdown<SavedSearch>
    {
        /// <summary>
        /// The maximum number of recently used saved searches shown in the dropdown.
        /// </summary>
        private const int MaxItems = 10;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Dropdown()
        {
        }

        /// <summary>
        /// Retrieves the recently used saved searches of the calling identity as dropdown items.
        /// </summary>
        /// <param name="query">The query (unused — the recents are resolved through the manager).</param>
        /// <param name="context">The query context.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The dropdown items, each running its saved search when selected.</returns>
        protected override IEnumerable<RestApiDropdownItem> RetrieveItems(IQuery<SavedSearch> query, IQueryContext context, IRequest request)
        {
            var ownerId = CoreHub.SessionManager.GetCurrentIdentityId(request);

            return CoreHub.SavedSearchManager.GetRecent(ownerId, MaxItems)
                .Select(x => new RestApiDropdownItem()
                {
                    Id = x.Id,
                    Text = x.Name,
                    Uri = RunUri(x)?.ToString()
                });
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
