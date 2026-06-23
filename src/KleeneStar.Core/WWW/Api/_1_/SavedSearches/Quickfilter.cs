using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.SavedSearches
{
    /// <summary>
    /// Provides the quickfilter options for the saved-search table — currently the
    /// "starred" chip.
    /// </summary>
    public sealed class Quickfilter : RestApiQuickfilter<Model.Entities.SavedSearch>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Quickfilter()
        {
        }

        /// <summary>
        /// Retrieves the quickfilter options.
        /// </summary>
        /// <param name="context">The query context (unused — the options are static).</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The quickfilter items.</returns>
        protected override IEnumerable<RestApiQuickfilterItem> RetrieveItems(IQueryContext context, IRequest request)
        {
            yield return new RestApiQuickfilterItem()
            {
                Id = "qf_starred",
                Name = I18N.Translate(request, "kleenestar.core:search.saved.quickfilter.starred")
            };
        }
    }
}
