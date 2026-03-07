using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WWW.Workspaces._workspacekey_;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Classes._workspacekey_
{
    /// <summary>
    /// Represents a REST API table for managing class entities, providing data retrieval 
    /// and option generation functionality for class records.
    /// </summary>
    [Title("kleenestar.core:class.tile.header")]
    [Cache]
    public sealed class Tile : RestApiTile<Class>
    {
        private readonly IUri _editFormUri;
        private readonly IUri _cloneFormUri;
        private readonly IUri _deleteFormUri;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Tile()
        {
            _editFormUri = CoreHub.GetUri<Edit>();
            _cloneFormUri = CoreHub.GetUri<Clone>();
            _deleteFormUri = CoreHub.GetUri<Delete>();
        }

        /// <summary>
        /// Retrieves a collection of options.
        /// </summary>
        /// <param name="row">
        /// The row object for which options are being retrieved. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The request object containing the criteria for retrieving options. Cannot be null.
        /// </param>
        public override IEnumerable<RestApiOption> GetOptions(Class row, IRequest request)
        {
            //var editUri = _editFormUri?
            //    .SetParameters(new KeyParameter(row.Key));
            //var cloneUri = _cloneFormUri?
            //    .SetParameters(new KeyParameter(row.Key));
            //var deleteUri = _deleteFormUri?
            //    .SetParameters(new KeyParameter(row.Key));

            //yield return new RestApiOptionHeader(request)
            //{
            //    Text = "webexpress.webapp:header.setting.label"
            //};

            //yield return new RestApiOptionEdit(request)
            //{
            //    PrimaryAction = new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge)
            //};

            //yield return new RestApiOptionClone(request)
            //{
            //    PrimaryAction = new ActionModal("modal-form", cloneUri, TypeModalSize.ExtraLarge)
            //};

            //yield return new RestApiOptionCustom(request)
            //{
            //    Uri = CoreHub.GetUri<WWW.Workspaces._key_.Classes.Index>()?
            //        .SetParameters
            //        (
            //            new KeyParameter(row.Key)
            //        ),
            //    Text = I18N.Translate(request, "kleenestar.core:class.manage.label"),
            //    Icon = new IconBoxesStacked().Class

            //};

            yield return new RestApiOptionSeparator(request);
            //yield return new RestApiOptionDelete(request)
            //{
            //    PrimaryAction = new ActionModal("modal-form", cloneUri, TypeModalSize.Small)
            //};
        }

        /// <summary>
        /// Creates a new instance of an object that implements the IQueryContext interface.
        /// </summary>
        /// <returns>
        /// An IQueryContext instance that can be used to execute queries.
        /// </returns>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
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
        /// An <see cref="IQueryable{TIndexItem}"/> representing the filtered set of index items. The 
        /// result may be empty if no items match the query.
        /// </returns>
        protected override IEnumerable<Class> Retrieve(IQuery<Class> query, IQueryContext context, IRequest request)
        {
            var key = request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(key?.Value);
            var id = workspace?.Id ?? Guid.Empty;

            query = query.WhereEquals(x => x.WorkspaceId, id);

            return CoreHub.ClassManager.GetClasses(query, context);
        }

        /// <summary>
        /// Applies the specified filter criteria to the given query object.
        /// </summary>
        /// <param name="filter">
        /// A string representing the filter expression to apply. The format and supported 
        /// operators depend on the implementation.
        /// </param>
        /// <param name="query">
        /// The query object to which the filter will be applied.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context for resolving
        /// the appropriate REST API URI.
        /// </param>
        /// <returns>
        /// A query representing the filtered set of items that match the criteria defined by 
        /// the filter statement.
        /// </returns>
        protected override IQuery<Class> Filter(string filter, IQuery<Class> query, IRequest request)
        {
            if (string.IsNullOrWhiteSpace(filter) || filter == "null")
            {
                return query;
            }

            query = query.WhereContainsIgnoreCase
            (
                x => x.Name, filter
            );

            //if (request.GetParameter<CategoryParameter>() is Parameter category)
            //{
            //    query = query.WhereContainsIgnoreCase
            //    (
            //        x => x.Categories.Select(x => x.Name),
            //        category.Value
            //    );
            //}

            return query;
        }
    }
}
