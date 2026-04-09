using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Core.WWW.Api._1_.Forms.Form._formid_
{
    /// <summary>
    /// Represents a REST API table endpoint that lists the field elements associated with a
    /// specific form. Each row corresponds to a field defined for the form's class, supporting
    /// the add-field and remove-field actions within the form designer.
    /// </summary>
    [Title("kleenestar.core:form.field.table.header")]
    [Cache]
    public sealed class Table : RestApiTable<Model.Entities.Field>
    {
        private readonly IUri _editFormUri;
        private readonly IUri _deleteFormUri;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Table()
        {
            _editFormUri = CoreHub.GetUri<Field._fieldid_.Edit>();
            _deleteFormUri = CoreHub.GetUri<Field._fieldid_.Delete>();
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
        /// Retrieves the collection of columns for the form field table.
        /// </summary>
        /// <param name="request">
        /// The request for which to retrieve the table columns. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of columns describing the form element table.
        /// </returns>
        protected override IEnumerable<RestApiTableColumn> RetrieveColums(IRequest request)
        {
            yield return new RestApiTableColumn()
            {
                Id = "name",
                Label = "kleenestar.core:form.field.name.label",
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "description",
                Label = "kleenestar.core:form.field.description.label",
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "state",
                Label = "kleenestar.core:form.field.state.label",
                Visible = false
            };
        }

        /// <summary>
        /// Retrieves the form field elements for the identified form. The rows are
        /// filtered to the class that owns the form so that only relevant fields are
        /// shown in the tab designer.
        /// </summary>
        /// <param name="query">
        /// The query that defines the criteria for selecting field rows.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed, providing additional
        /// information or constraints.
        /// </param>
        /// <param name="columns">
        /// The collection of columns to include in the result set.
        /// </param>
        /// <param name="request">
        /// The request object containing metadata or parameters relevant to the
        /// retrieval operation.
        /// </param>
        /// <returns>
        /// An enumerable collection of table rows representing the form's field elements.
        /// </returns>
        protected override IEnumerable<RestApiTableRow> RetrieveRows(IQuery<Model.Entities.Field> query, IQueryContext context, IEnumerable<RestApiTableColumn> columns, IRequest request)
        {
            var formIdParam = request.GetParameter<FormIdParameter>();
            var guid = Guid.TryParse(formIdParam?.Value, out Guid id) ? id : Guid.Empty;
            var form = CoreHub.FormManager.GetForm(guid);

            if (form is null)
            {
                return [];
            }

            query = query.WhereEquals(x => x.ClassId, form.ClassId);

            return CoreHub.FieldManager.GetFields(query, context)
                .Select(x => new RestApiTableRow
                {
                    Id = x.Id.ToString(),
                    Cells =
                    [
                        new RestApiTableCell() { Content = x.Name },
                        new RestApiTableCell() { Content = x.Description },
                        new RestApiTableCell() { Content = x.State.ToString() }
                    ],
                    Options = GetOptions(x, request).Select(o => o.ToJson())
                });
        }

        /// <summary>
        /// Applies the specified search filter to the field query.
        /// </summary>
        /// <param name="filter">
        /// A string representing the filter expression to apply.
        /// </param>
        /// <param name="query">
        /// The query object to which the filter will be applied.
        /// </param>
        /// <param name="request">
        /// The request that provides the operational context.
        /// </param>
        /// <returns>
        /// A query representing the filtered set of fields.
        /// </returns>
        protected override IQuery<Model.Entities.Field> Filter(string filter, IQuery<Model.Entities.Field> query, IRequest request)
        {
            if (string.IsNullOrWhiteSpace(filter) || filter == "null")
            {
                return query;
            }

            return query.WhereContainsIgnoreCase(x => x.Name, filter);
        }

        /// <summary>
        /// Retrieves contextual options for a field row (edit, delete).
        /// </summary>
        /// <param name="row">
        /// The field for which options are being retrieved. Cannot be null.
        /// </param>
        /// <param name="request">
        /// The current HTTP request. Cannot be null.
        /// </param>
        private IEnumerable<RestApiOption> GetOptions(Model.Entities.Field row, IRequest request)
        {
            var editUri = _editFormUri?
                .BindParameters(request)
                .BindParameters(new FieldIdParameter(row.Id));
            var deleteUri = _deleteFormUri?
                .BindParameters(request)
                .BindParameters(new FieldIdParameter(row.Id));

            yield return new RestApiOptionHeader(request)
            {
                Text = "webexpress.webapp:header.setting.label"
            };

            yield return new RestApiOptionEdit(request)
            {
                PrimaryAction = new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge)
            };

            yield return new RestApiOptionSeparator(request);

            yield return new RestApiOptionDelete(request)
            {
                PrimaryAction = new ActionModal("modal-form", deleteUri, TypeModalSize.Small)
            };
        }
    }
}
