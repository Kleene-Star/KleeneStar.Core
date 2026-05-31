using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Fields
{
    /// <summary>
    /// Represents a selectable field type for use in REST API selection scenarios.
    /// </summary>
    [Title("Field type")]
    public sealed class FieldType : RestApiSelection<Model.Entities.Field>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FieldType()
        {
        }

        /// <summary>
        /// Retrieves a queryable collection of index items that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select index items. Cannot 
        /// be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// An enumerable collection of selection items that satisfy the query 
        /// criteria. The collection is empty if no items match.
        /// </returns>
        protected override IQuery<Model.Entities.Field> Filter(string filter, IQuery<Model.Entities.Field> query, IRequest request)
        {
            return query;
        }

        /// <summary>
        /// Retrieves a queryable collection of index items that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// An object containing the query parameters used to filter and select index items. Cannot 
        /// be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// An enumerable collection of selection items that satisfy the query 
        /// criteria. The collection is empty if no items match.
        /// </returns>
        protected override IQueryable<RestApiSelectionItem> RetrieveItems(IQuery<Model.Entities.Field> query, IQueryContext context, IRequest request)
        {
            var list = new List<RestApiSelectionItem>()
            {
                new()
                {
                    Id = Model.Entities.FieldType.Text.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Text.Text()),
                    Color = Model.Entities.FieldType.Text.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.Number.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Number.Text()),
                    Color = Model.Entities.FieldType.Number.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.Date.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Date.Text()),
                    Color = Model.Entities.FieldType.Date.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.Boolean.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Boolean.Text()),
                    Color = Model.Entities.FieldType.Boolean.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.Selection.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Selection.Text()),
                    Color = Model.Entities.FieldType.Selection.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.Reference.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Reference.Text()),
                    Color = Model.Entities.FieldType.Reference.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.Workflow.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Workflow.Text()),
                    Color = Model.Entities.FieldType.Workflow.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.Attachment.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Attachment.Text()),
                    Color = Model.Entities.FieldType.Attachment.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.User.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.User.Text()),
                    Color = Model.Entities.FieldType.User.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.Tag.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Tag.Text()),
                    Color = Model.Entities.FieldType.Tag.Color()
                },
                new()
                {
                    Id = Model.Entities.FieldType.Priority.Id(),
                    Text = I18N.Translate(request, Model.Entities.FieldType.Priority.Text()),
                    Color = Model.Entities.FieldType.Priority.Color()
                }
            };

            return list.AsQueryable();
        }
    }
}
