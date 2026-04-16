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
                new() { Id = Guid.Parse("D4E8EFC6-9A56-4AAC-9FCF-E924F46CB7E5"), Text = "Private" },
                new() { Id = Guid.Parse("04505D9D-F7EF-4CF9-9775-FA141C59BA95"), Text = "Protected" },
                new() { Id = Guid.Parse("7530F626-A0BE-444F-99C4-F1548387E6D3"), Text = "Public" },
                new() { Id = Guid.Parse("12419EF5-6426-43B2-A66B-CCEFD64BD23B"), Text = "Internal" }
            }.AsQueryable();
        }
    }
}
