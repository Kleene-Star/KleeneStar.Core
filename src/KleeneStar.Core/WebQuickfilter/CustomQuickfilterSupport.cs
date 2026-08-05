using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex;
using WebExpress.WebIndex.Queries;
using WebExpress.WebIndex.Wql;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebQuickfilter
{
    /// <summary>
    /// Adds the quickfilters a user defined to the bar of a view and applies them to its query.
    /// </summary>
    /// <remarks>
    /// Every view wires custom quickfilters in the same two places — the quickfilter endpoint that
    /// fills the bar and the table endpoint that answers with the rows — so both halves live here
    /// rather than being written out per view. Adding a further view is then a matter of calling
    /// <see cref="Items"/> and <see cref="Apply"/> with that view's key.
    /// </remarks>
    public static class CustomQuickfilterSupport
    {
        /// <summary>
        /// Returns the stored quickfilters of a view as items for its bar.
        /// </summary>
        /// <param name="viewKey">The view whose bar is being filled.</param>
        /// <param name="contextKey">
        /// The context that narrows the view, or null for a view that exists only once.
        /// </param>
        /// <param name="request">The request the bar is rendered for.</param>
        /// <returns>
        /// The items to append after the view's own chips. Empty when the user has defined none.
        /// </returns>
        public static IEnumerable<RestApiQuickfilterItem> Items(string viewKey, string contextKey, IRequest request)
        {
            var identityId = CoreHub.SessionManager.GetCurrentIdentityId(request);

            foreach (var filter in CoreHub.CustomQuickfilterManager.GetVisibleCustomQuickfilters(viewKey, contextKey, identityId))
            {
                yield return new RestApiQuickfilterItem()
                {
                    Id = filter.FilterId,
                    // the name is what the user typed, so it is offered as written rather than
                    // resolved as a translation key
                    Name = filter.Name,
                    // a shared filter is marked, because the bar otherwise gives no clue why a chip
                    // the user never created is being offered
                    Icon = filter.Shared ? new IconUsers() : new IconFilter()
                };
            }
        }

        /// <summary>
        /// Applies the stored quickfilters among the active chips to a query.
        /// </summary>
        /// <remarks>
        /// The filters are composed onto the running query rather than replacing it, so a stored
        /// filter combines with the view's own chips and with the search term instead of discarding
        /// them. A filter whose expression no longer parses is skipped: the view stays usable and
        /// the remaining chips keep working, which matters because the expression was typed by hand
        /// and the fields it names can be removed later.
        /// </remarks>
        /// <typeparam name="TIndexItem">The type the view lists.</typeparam>
        /// <param name="filters">The quickfilter ids reported as active.</param>
        /// <param name="query">The query to narrow.</param>
        /// <param name="viewKey">The view the filters must belong to.</param>
        /// <returns>The narrowed query.</returns>
        public static IQuery<TIndexItem> Apply<TIndexItem>(IEnumerable<string> filters, IQuery<TIndexItem> query, string viewKey)
            where TIndexItem : IIndexItem
        {
            foreach (var predicate in Predicates<TIndexItem>(filters, viewKey))
            {
                query = query.Where(predicate);
            }

            return query;
        }

        /// <summary>
        /// Applies the stored quickfilters among the active chips to a materialized sequence.
        /// </summary>
        /// <remarks>
        /// Views that answer from memory rather than from a query — because their rows are composed
        /// from several sources — narrow the sequence instead. The stored expression is the same
        /// one either way, only compiled here rather than handed to the query.
        /// </remarks>
        /// <typeparam name="TIndexItem">The type the view lists.</typeparam>
        /// <param name="filters">The quickfilter ids reported as active.</param>
        /// <param name="items">The sequence to narrow.</param>
        /// <param name="viewKey">The view the filters must belong to.</param>
        /// <returns>The narrowed sequence.</returns>
        public static IEnumerable<TIndexItem> Apply<TIndexItem>(IEnumerable<string> filters, IEnumerable<TIndexItem> items, string viewKey)
            where TIndexItem : IIndexItem
        {
            foreach (var predicate in Predicates<TIndexItem>(filters, viewKey))
            {
                items = items.Where(predicate.Compile());
            }

            return items;
        }

        /// <summary>
        /// Returns the conditions of the stored quickfilters among the active chips.
        /// </summary>
        /// <remarks>
        /// A filter of another view is skipped even when its id is passed in, because its
        /// expression names fields this type does not have.
        /// </remarks>
        /// <typeparam name="TIndexItem">The type the view lists.</typeparam>
        /// <param name="filters">The quickfilter ids reported as active.</param>
        /// <param name="viewKey">The view the filters must belong to.</param>
        /// <returns>One condition per applicable filter.</returns>
        private static IEnumerable<Expression<Func<TIndexItem, bool>>> Predicates<TIndexItem>(IEnumerable<string> filters, string viewKey)
            where TIndexItem : IIndexItem
        {
            foreach (var filterId in filters ?? [])
            {
                var id = CustomQuickfilter.ParseFilterId(filterId);

                if (id is null)
                {
                    continue;
                }

                var filter = CoreHub.CustomQuickfilterManager.GetCustomQuickfilter(id.Value);

                if (filter is null || !string.Equals(filter.ViewKey, viewKey, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var predicate = Compile<TIndexItem>(filter.Query);

                if (predicate is not null)
                {
                    yield return predicate;
                }
            }
        }

        /// <summary>
        /// Turns a WQL expression into a condition.
        /// </summary>
        /// <remarks>
        /// The statement's own <c>ToQuery</c> starts from an empty query and would drop everything
        /// applied so far, so only its filter condition is taken and left to the caller to add to
        /// whatever it is already narrowing.
        /// </remarks>
        /// <typeparam name="TIndexItem">The type the view lists.</typeparam>
        /// <param name="wql">The expression to compile.</param>
        /// <returns>
        /// The condition, or null when the expression is empty, carries none, or no longer parses.
        /// </returns>
        private static Expression<Func<TIndexItem, bool>> Compile<TIndexItem>(string wql)
            where TIndexItem : IIndexItem
        {
            if (string.IsNullOrWhiteSpace(wql))
            {
                return null;
            }

            try
            {
                var statement = new WqlParser<TIndexItem>().Parse(wql);

                if (statement is null || statement.HasErrors || statement.Filter is null)
                {
                    return null;
                }

                var param = Expression.Parameter(typeof(TIndexItem), "x");
                var body = statement.Filter.ToExpression(param);

                return Expression.Lambda<Func<TIndexItem, bool>>(body, param);
            }
            catch
            {
                // a stored expression that no longer parses must not take the whole view down
                return null;
            }
        }
    }
}
