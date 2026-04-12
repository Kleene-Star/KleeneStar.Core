using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Identities
{
    /// <summary>
    /// Represents selectable identity states for REST API selection.
    /// </summary>
    [Title("Identity state")]
    public sealed class State : RestApiSelection<Model.Entities.Identity>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public State()
        {
        }

        /// <summary>
        /// Retrieves selection items.
        /// </summary>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Model.Entities.Identity> query, IQueryContext context, IRequest request)
        {
            var list = new List<RestApiSelectionItem>()
            {
                new()
                {
                    Id = Guid.Empty,
                    Text = "Active"
                },
                new()
                {
                    Id = Guid.Empty,
                    Text = "Inactive"
                }
            };

            return list.AsQueryable();
        }

        /// <summary>
        /// Applies filters.
        /// </summary>
        protected override IQuery<Model.Entities.Identity> Filter(string filter, IQuery<Model.Entities.Identity> query, IRequest request)
        {
            if (filter is null || filter == "null")
            {
                return query;
            }

            return query.WhereContainsIgnoreCase
            (
                x => x.Name, filter
            );
        }
    }
}
