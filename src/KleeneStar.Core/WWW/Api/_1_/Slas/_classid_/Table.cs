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

namespace KleeneStar.Core.WWW.Api._1_.Slas._classid_
{
    /// <summary>
    /// Backs the SLA-policy table view: emits the columns, rows and per-row options
    /// (edit/clone/delete) used by the SLA overview page.
    /// </summary>
    [Title("kleenestar.core:sla.table.header")]
    [Cache]
    public sealed class Table : RestApiTable<SlaPolicy>
    {
        private readonly IUri _editFormUri;
        private readonly IUri _cloneFormUri;
        private readonly IUri _deleteFormUri;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Table()
        {
            _editFormUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Sla._slaid_.Edit>();
            _cloneFormUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Sla._slaid_.Clone>();
            _deleteFormUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Sla._slaid_.Delete>();
        }

        /// <inheritdoc/>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <inheritdoc/>
        protected override IEnumerable<RestApiTableColumn> RetrieveColums(IRequest request)
        {
            yield return new RestApiTableColumn { Id = "name",     Label = "Name",     Visible = true };
            yield return new RestApiTableColumn { Id = "priority", Label = "Priority", Visible = true };
            yield return new RestApiTableColumn { Id = "calendar", Label = "Calendar", Visible = true };
            yield return new RestApiTableColumn { Id = "state",    Label = "State",    Visible = true };
            yield return new RestApiTableColumn { Id = "targets",  Label = "Targets",  Visible = true };
            yield return new RestApiTableColumn { Id = "updated",  Label = "Updated",  Visible = false };
        }

        /// <inheritdoc/>
        protected override IEnumerable<RestApiTableRow> RetrieveRows(IQuery<SlaPolicy> query, IQueryContext context, IEnumerable<RestApiTableColumn> columns, IRequest request)
        {
            var classId = request.GetParameter<ClassIdParameter>();
            var guid = Guid.TryParse(classId?.Value, out var id) ? id : Guid.Empty;

            query = query.WhereEquals(x => x.ClassId, guid);

            return CoreHub.SlaManager.GetSlas(query, context)
                .Select(x => new RestApiTableRow
                {
                    Id = x.Id.ToString(),
                    Cells =
                    [
                        new RestApiTableCell { Content = x.Name },
                        new RestApiTableCell { Content = x.Priority.ToString() },
                        new RestApiTableCell { Content = x.Calendar.ToString() },
                        new RestApiTableCell { Content = x.State.ToString() },
                        new RestApiTableCell { Content = (x.Targets?.Count ?? 0).ToString() },
                        new RestApiTableCell { Content = x.Updated.ToString("u") }
                    ],
                    Options = GetOptions(x, request).Select(o => o.ToJson()),
                    Uri = GetUri(x, request)?.ToString()
                });
        }

        /// <inheritdoc/>
        protected override IQuery<SlaPolicy> Filter(string filter, IQuery<SlaPolicy> query, IRequest request)
        {
            if (string.IsNullOrWhiteSpace(filter) || filter == "null")
            {
                return query;
            }

            return query.WhereContainsIgnoreCase(x => x.Name, filter);
        }

        /// <inheritdoc/>
        protected override IQuery<SlaPolicy> Filter(IEnumerable<string> filters, IQuery<SlaPolicy> query, IRequest request)
        {
            foreach (var filter in filters.Where(f => f.StartsWith("qf_", StringComparison.OrdinalIgnoreCase)))
            {
                var key = filter[3..];

                switch (key.ToLowerInvariant())
                {
                    case "active":
                        query = query.Where(x => x.State == SlaPolicyState.Active);
                        break;
                    case "draft":
                        query = query.Where(x => x.State == SlaPolicyState.Draft);
                        break;
                    case "inactive":
                        query = query.Where(x => x.State == SlaPolicyState.Inactive);
                        break;
                    case "atrisk":
                        query = query.Where(x => x.Priority == SlaPriority.Critical);
                        break;
                    default:
                        continue;
                }
            }

            return query;
        }

        /// <summary>
        /// Emits the per-row option entries (header, edit, clone, separator, delete).
        /// </summary>
        private IEnumerable<RestApiOption> GetOptions(SlaPolicy row, IRequest request)
        {
            var editUri = _editFormUri?
                .BindParameters(request)
                .BindParameters(new SlaIdParameter(row.Id));
            var cloneUri = _cloneFormUri?
                .BindParameters(request)
                .BindParameters(new SlaIdParameter(row.Id));
            var deleteUri = _deleteFormUri?
                .BindParameters(request)
                .BindParameters(new SlaIdParameter(row.Id));

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
        /// Returns the URI a row click navigates to. Returning <c>null</c> keeps the row inert.
        /// </summary>
        private static IUri GetUri(SlaPolicy row, IRequest request)
        {
            return null;
        }
    }
}
