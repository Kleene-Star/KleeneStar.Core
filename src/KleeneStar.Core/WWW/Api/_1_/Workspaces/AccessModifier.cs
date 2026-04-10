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
                new() { Id = Guid.Empty, Text = "Private" },
                new() { Id = Guid.Empty, Text = "Protected" },
                new() { Id = Guid.Empty, Text = "Public" },
                new() { Id = Guid.Empty, Text = "Internal" }
            }.AsQueryable();
        }
    }
}
