using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Slas
{
    /// <summary>
    /// REST selection of the available <see cref="SlaPriority"/> values.
    /// </summary>
    [Title("SLA priority")]
    public sealed class Priority : RestApiSelection<SlaPolicy>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Priority()
        {
        }

        /// <inheritdoc/>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<SlaPolicy> query, IQueryContext context, IRequest request)
        {
            var list = Enum.GetValues<SlaPriority>()
                .Select(p => new RestApiSelectionItem
                {
                    Id = Guid.NewGuid(),
                    Text = I18N.Translate(request, $"kleenestar.core:sla.priority.{p.ToString().ToLowerInvariant()}.label")
                })
                .ToList();

            return list.AsQueryable();
        }

        /// <inheritdoc/>
        protected override IQuery<SlaPolicy> Filter(string filter, IQuery<SlaPolicy> query, IRequest request)
        {
            return query;
        }
    }
}
