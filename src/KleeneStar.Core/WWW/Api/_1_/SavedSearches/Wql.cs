using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.SavedSearches
{
    // The entity type SavedSearch collides with the sibling WWW.SavedSearch namespace;
    // alias it (inside the namespace block) so the bare name binds to the entity.
    using SavedSearch = KleeneStar.Model.Entities.SavedSearch;

    /// <summary>
    /// Backs the advanced-search prompt of the saved-search sidebar table.
    /// </summary>
    [Cache]
    public sealed class Wql : RestApiWqlPrompt<SavedSearch>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Wql()
        {
        }

        /// <summary>
        /// Provides a small set of example queries shown in the prompt history.
        /// </summary>
        /// <param name="request">The request for which to retrieve history.</param>
        /// <returns>The example query history entries.</returns>
        protected override IEnumerable<string> GetHistory(IRequest request)
        {
            yield return "Name ~ \"incident\"";
            yield return "Starred = true";
        }
    }
}
