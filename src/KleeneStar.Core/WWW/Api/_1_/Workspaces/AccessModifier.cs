using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Workspaces
{
    /// <summary>
    /// Represents a selectable access modifier for workspace editing.
    /// </summary>
    [Title("Workspace access modifier")]
    public sealed class AccessModifier : RestApiSelection<Model.Entities.Workspace>
    {
        /// <summary>
        /// Applies the specified filter criteria to the given query object.
        /// </summary>
        protected override IQuery<Model.Entities.Workspace> Filter(string filter, IQuery<Model.Entities.Workspace> query, IRequest request)
        {
            return query;
        }

        /// <summary>
        /// Retrieves the available access modifier values.
        /// </summary>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Model.Entities.Workspace> query, IQueryContext context, IRequest request)
        {
            return new List<RestApiSelectionItem>()
            {
                new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Text = "Private" },
                new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Text = "Protected" },
                new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Text = "Public" },
                new() { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Text = "Internal" }
            }.AsQueryable();
        }
    }
}
