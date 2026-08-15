using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Profile
{
    /// <summary>
    /// Serves the languages the user interface is offered in to the selection on the account
    /// page.
    /// </summary>
    [Title("Interface language")]
    public sealed class Language : RestApiSelection<Model.Entities.Identity>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Language()
        {
        }

        /// <summary>
        /// Retrieves the selectable languages, each named in the language itself so it stays
        /// recognizable no matter which one the interface currently runs in.
        /// </summary>
        /// <param name="query">The query criteria; paging is applied by the base class.</param>
        /// <param name="context">The context in which the query is executed.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The selectable languages.</returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems
        (
            IQuery<Model.Entities.Identity> query,
            IQueryContext context,
            IRequest request
        )
        {
            return UiLanguage.All
                .Select(x => new RestApiSelectionItem
                {
                    Id = x.Id,
                    Text = x.Label
                })
                .AsQueryable();
        }

        /// <summary>
        /// Applies the search term typed into the selection.
        /// </summary>
        /// <param name="filter">The search term, or null when nothing was typed.</param>
        /// <param name="query">The query to narrow.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The narrowed query.</returns>
        protected override IQuery<Model.Entities.Identity> Filter
        (
            string filter,
            IQuery<Model.Entities.Identity> query,
            IRequest request
        )
        {
            // the catalogue is short and is built in RetrieveItems rather than queried, so
            // there is nothing to narrow here
            return query;
        }
    }
}
