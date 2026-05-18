using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Api._1_.Templates._workspacekey_
{
    /// <summary>
    /// Represents a REST API table for managing template entities, providing data retrieval
    /// and option generation functionality for template records.
    /// </summary>
    [Title("kleenestar.core:template.table.header")]
    [Cache]
    public sealed class Table : RestApiTable<Model.Entities.Template>
    {
        private readonly IUri _editFormUri;
        private readonly IUri _cloneFormUri;
        private readonly IUri _deleteFormUri;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Table()
        {
            _editFormUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Template._templateid_.Edit>();
            _cloneFormUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Template._templateid_.Clone>();
            _deleteFormUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Template._templateid_.Delete>();
        }

        /// <summary>
        /// Creates a new instance of an object that implements the IQueryContext interface.
        /// </summary>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Retrieves the collection of columns for the specified REST API request.
        /// </summary>
        protected override IEnumerable<RestApiTableColumn> RetrieveColums(IRequest request)
        {
            yield return new RestApiTableColumn()
            {
                Id = "name",
                Label = "Name",
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "description",
                Label = "Description",
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "category",
                Label = "Category",
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "state",
                Label = "State",
                Visible = false
            };
        }

        /// <summary>
        /// Retrieves a collection of table rows that match the specified query and context.
        /// </summary>
        protected override IEnumerable<RestApiTableRow> RetrieveRows(IQuery<Model.Entities.Template> query, IQueryContext context, IEnumerable<RestApiTableColumn> columns, IRequest request)
        {
            return CoreHub.TemplateManager.GetTemplates(query, context)
                .Select(x => new RestApiTableRow
                {
                    Id = x.Id.ToString(),
                    Cells =
                    [
                        new RestApiTableCell() {
                             Content = x.Name
                        },
                        new() {
                            Content = x.Description
                        },
                        new() {
                            Content = x.Category
                        },
                        new() {
                            Content = x.State.ToString()
                        }
                    ],
                    Options = GetOptions(x, request).Select(o => o.ToJson()),
                    Uri = GetUri(x, request)?.ToString()
                });
        }

        /// <summary>
        /// Applies the specified filter criteria to the given query object.
        /// </summary>
        protected override IQuery<Model.Entities.Template> Filter(string filter, IQuery<Model.Entities.Template> query, IRequest request)
        {
            if (string.IsNullOrWhiteSpace(filter) || filter == "null")
            {
                return query;
            }

            return query.WhereContainsIgnoreCase
            (
                x => x.Name, filter
            );
        }

        /// <summary>
        /// Applies the specified quick filter criteria to the given query object.
        /// </summary>
        protected override IQuery<Model.Entities.Template> Filter(IEnumerable<string> filters, IQuery<Model.Entities.Template> query, IRequest request)
        {
            foreach (var filter in filters.Where(f => f.StartsWith("qf_", StringComparison.OrdinalIgnoreCase)))
            {
                var key = filter[3..];

                switch (key.ToLowerInvariant())
                {
                    case "active":
                        query = query.Where(x => x.State == TemplateState.Active);
                        break;
                    default:
                        continue;
                }
            }

            return query;
        }

        /// <summary>
        /// Retrieves a collection of options for the row.
        /// </summary>
        private IEnumerable<RestApiOption> GetOptions(Model.Entities.Template row, IRequest request)
        {
            var editUri = _editFormUri?
                .BindParameters(request)
                .BindParameters(new TemplateIdParameter(row.Id));
            var cloneUri = _cloneFormUri?
                .BindParameters(request)
                .BindParameters(new TemplateIdParameter(row.Id));
            var deleteUri = _deleteFormUri?
                .BindParameters(request)
                .BindParameters(new TemplateIdParameter(row.Id));

            var iconTheme = request?.ApplicationContext?.DefaultTheme?.IconTheme ?? TypeIconTheme.Light;

            yield return new RestApiOptionHeader(request)
            {
                Text = "webexpress.webapp:header.setting.label"
            };

            yield return new RestApiOptionEdit(request)
            {
                Icon = new IconPen(iconTheme),
                PrimaryAction = new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge)
            };

            yield return new RestApiOptionClone(request)
            {
                Icon = new IconClone(iconTheme),
                PrimaryAction = new ActionModal("modal-form", cloneUri, TypeModalSize.ExtraLarge)
            };

            yield return new RestApiOptionSeparator(request);
            yield return new RestApiOptionDelete(request)
            {
                Icon = new IconTrash(iconTheme),
                PrimaryAction = new ActionModal("modal-form", deleteUri, TypeModalSize.Small)
            };
        }

        /// <summary>
        /// Retrieves a URI that represents the specified template row.
        /// </summary>
        private static IUri GetUri(Model.Entities.Template row, IRequest request)
        {
            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Template._templateid_.Edit>()
                .BindParameters(new TemplateIdParameter(row.Id));

            return uri;
        }
    }
}
