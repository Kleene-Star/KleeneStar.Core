using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebQuickfilter;
using KleeneStar.Core.WebRestApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;
using WebExpress.WebCore.WebStatusPage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;

// The entity type Object collides with System.Object; alias it so the
// projection code reads naturally.
using ObjectEntity = KleeneStar.Model.Entities.Object;

namespace KleeneStar.Core.WWW.Api._1_.Issues._workspacekey_
{
    /// <summary>
    /// Table endpoint of the issue overview: returns the issue-kind objects of the
    /// addressed workspace, filtered by the search term and the personal quickfilters
    /// (starred, assigned to me, created by me, archived), sorted, and paged.
    /// </summary>
    /// <remarks>
    /// The columns are not fixed. Every field of every issue class of the workspace is
    /// offered as a column alongside the object's own properties (see
    /// <see cref="ObjectTableColumnCatalog"/>), and which of them a table shows, in what
    /// order and at what width, is the choice of the user looking at it. That choice is
    /// stored per identity in the <c>UserSession</c> table through
    /// <see cref="WebManager.ISessionManager"/>, and it is stored per view: the overview
    /// hosts several tabs backed by this endpoint, and the <c>v</c> parameter in the
    /// address of each tab's table (put there by
    /// <see cref="Objects._workspacekey_.Tab"/>) tells them apart, so an "Issues" tab and
    /// a "Table" tab of the same workspace keep separate column sets.
    ///
    /// Starring is deliberately not a column: it is a mark on the row, so a starred issue
    /// shows a star beside its key and is toggled from the row menu.
    ///
    /// The endpoint is a plain <see cref="IRestApi"/> rather than a
    /// <c>RestApiTable&lt;Object&gt;</c> because the starred scope is a per-identity
    /// projection (the favorite flag lives on the caller's visit rows) that a WebIndex
    /// query cannot express — the base class would page the wrong set. Filtering, sorting
    /// and paging therefore run in memory over the workspace's issues. The endpoint
    /// honours the same query parameters the other REST tables use: <c>q</c> (substring
    /// search), <c>f</c> (comma-separated quickfilter ids), <c>p</c> (zero-based page
    /// number), <c>l</c> (page size), <c>o</c> (order column id) and <c>d</c> (order
    /// direction).
    /// </remarks>
    [Cache]
    public sealed class Table : IRestApi
    {
        /// <summary>
        /// The default page size used when the request carries no (or an invalid)
        /// <c>l</c> parameter.
        /// </summary>
        private const int DefaultPageSize = 50;

        /// <summary>
        /// The request parameter naming the view whose column layout applies.
        /// </summary>
        private const string ViewParameter = "v";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Table()
        {
        }

        /// <summary>
        /// Builds the filtered, sorted, paged page of the workspace's issues and returns
        /// it as a table result response.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <returns>The table result as a JSON response.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Get(IRequest request)
        {
            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceKey);
            var ownerId = CoreHub.SessionManager.GetCurrentIdentityId(request);

            var pageNumber = Math.Max(0, ParseInt(request, "p", 0));
            var pageSize = ParseInt(request, "l", DefaultPageSize);
            if (pageSize <= 0)
            {
                pageSize = DefaultPageSize;
            }

            var search = request?.GetParameter("q")?.Value;
            var filters = request?.GetParameter("f")?.Value?
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) ?? [];
            var selected = new HashSet<string>(filters, StringComparer.OrdinalIgnoreCase);

            var starredIds = CoreHub.ObjectManager.GetFavoriteObjects(ownerId)
                .Select(x => x.Id)
                .ToHashSet();

            var issues = GetIssues(workspace?.Id).AsEnumerable();

            // the archived chip flips the lifecycle scope: without it the list shows the
            // active issues, with it the archived history
            var state = selected.Contains(Quickfilter.ArchivedId)
                ? Model.Entities.WorkspaceState.Archived
                : Model.Entities.WorkspaceState.Active;
            issues = issues.Where(x => x.State == state);

            if (selected.Contains(Quickfilter.StarredId))
            {
                issues = issues.Where(x => starredIds.Contains(x.Id));
            }

            if (selected.Contains(Quickfilter.MineId))
            {
                issues = issues.Where(x => x.AssigneeId == ownerId);
            }

            if (selected.Contains(Quickfilter.CreatedId))
            {
                issues = issues.Where(x => x.CreatorId == ownerId);
            }

            if (!string.IsNullOrWhiteSpace(search) && search != "null")
            {
                issues = issues.Where(x =>
                    (x.Key ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (x.Summary ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (x.Description ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // the filters the user defined are resolved from storage rather than from a chip id
            // handled above, and narrow further so they combine with the scopes and the search
            issues = CustomQuickfilterSupport.Apply(filters, issues, Quickfilter.ViewKey);

            var catalog = ObjectTableColumnCatalog.Build(workspace?.Id, Model.Entities.ObjectKind.Issue, request);
            var layout = ResolveLayout(catalog, request);

            var filtered = issues.ToList();
            var page = Sort(filtered, layout, request)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToList();

            // the values and class definitions of the whole page are read in one go, so a
            // table with twenty field columns does not issue a query per cell
            var projection = ObjectTableProjection.Build(page);

            var result = new RestApiTableResult()
            {
                Title = null,
                Columns = layout.Select(x => x.Column.ToRestApiColumn(x.Visible, x.Width)),
                Rows = page.Select(x => BuildRow(x, layout, projection, starredIds.Contains(x.Id), request)),
                Pagination = new RestApiPaginationInfo()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = filtered.Count
                }
            };

            return result.ToResponse();
        }

        /// <summary>
        /// Handles the column layout the user configured in the table's column manager:
        /// the visible set, their order and their widths, stored against the calling
        /// identity and the addressed view.
        /// </summary>
        /// <remarks>
        /// The client sends the same payload it sends to <c>RestApiTable.Configure</c>
        /// (<c>{ "c": [{ "id", "visible", "width" }, …] }</c>) plus, on a row reorder, a
        /// row id list under <c>r</c>. The issue table sorts by a column rather than by a
        /// stored row sequence, so the row list is accepted and ignored.
        /// </remarks>
        /// <param name="request">The incoming request.</param>
        /// <returns><c>204</c> once stored, or <c>400</c> for an unreadable payload.</returns>
        [Method(RequestMethod.PUT)]
        public IResponse Configure(IRequest request)
        {
            var payload = ReadConfigurePayload(request);

            if (payload?.Columns is not { Count: > 0 })
            {
                return new ResponseBadRequest(new StatusMessage("Missing column configuration."));
            }

            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value;
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceKey);
            var catalog = ObjectTableColumnCatalog.Build(workspace?.Id, Model.Entities.ObjectKind.Issue, request);
            var known = catalog.Columns.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var snapshot = new List<RestApiTableColumn>(known.Count);

            // the client reports the columns it currently holds; an id it does not know
            // (a field deleted meanwhile, or a stale layout) is dropped rather than stored
            foreach (var update in payload.Columns)
            {
                if (string.IsNullOrWhiteSpace(update?.Id) ||
                    !known.TryGetValue(update.Id, out var column) ||
                    !seen.Add(column.Id))
                {
                    continue;
                }

                snapshot.Add(column.ToRestApiColumn(update.Visible ?? column.DefaultVisible, update.Width));
            }

            if (snapshot.Count == 0)
            {
                return new ResponseBadRequest(new StatusMessage("No known column in the configuration."));
            }

            CoreHub.SessionManager.SetTableLayout(request, ResolveLayoutKey(request), snapshot);

            return new ResponseNoContent();
        }

        /// <summary>
        /// Returns the key the calling identity's column layout for the addressed view is
        /// stored under. The view is named by the <c>v</c> parameter; a table addressed
        /// without one (a direct call, or a tab whose view could not be resolved) falls
        /// back to a shared default, so it still remembers a layout rather than none.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <returns>The layout key.</returns>
        private static string ResolveLayoutKey(IRequest request)
        {
            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value ?? string.Empty;
            var view = request?.GetParameter(ViewParameter)?.Value;

            if (string.IsNullOrWhiteSpace(view) || view == "null")
            {
                view = "default";
            }

            return $"{typeof(Table).FullName}:{workspaceKey}:{view}";
        }

        /// <summary>
        /// Lays the stored per-identity, per-view layout over the catalog: the stored
        /// columns come first in their stored order with their stored visibility and
        /// width, and every column the layout does not mention follows, hidden — a field
        /// added to a class after the user configured the table is offered in the column
        /// manager without forcing itself into the table.
        /// </summary>
        /// <param name="catalog">The columns the table can offer.</param>
        /// <param name="request">The incoming request.</param>
        /// <returns>The effective columns in display order.</returns>
        private static IReadOnlyList<ObjectTableColumnState> ResolveLayout(ObjectTableColumnCatalog catalog, IRequest request)
        {
            var stored = CoreHub.SessionManager.GetTableLayout(request, ResolveLayoutKey(request));

            if (stored is null || stored.Count == 0)
            {
                return
                [
                    .. catalog.Columns.Select(x => new ObjectTableColumnState
                    {
                        Column = x,
                        Visible = x.DefaultVisible
                    })
                ];
            }

            var known = catalog.Columns.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var layout = new List<ObjectTableColumnState>(known.Count);

            foreach (var entry in stored)
            {
                if (string.IsNullOrWhiteSpace(entry?.Id) ||
                    !known.TryGetValue(entry.Id, out var column) ||
                    !seen.Add(column.Id))
                {
                    continue;
                }

                layout.Add(new ObjectTableColumnState
                {
                    Column = column,
                    Visible = entry.Visible ?? column.DefaultVisible,
                    Width = entry.Width
                });
            }

            layout.AddRange(catalog.Columns
                .Where(x => !seen.Contains(x.Id))
                .Select(x => new ObjectTableColumnState
                {
                    Column = x,
                    Visible = false
                }));

            return layout;
        }

        /// <summary>
        /// Orders the issues by the column the client asked for, falling back to the most
        /// recently updated first. The comparison runs over the cell content of the
        /// column, so a table sorts by what it shows.
        /// </summary>
        /// <param name="issues">The filtered issues.</param>
        /// <param name="layout">The effective columns.</param>
        /// <param name="request">The incoming request.</param>
        /// <returns>The ordered issues.</returns>
        private static IEnumerable<ObjectEntity> Sort
        (
            IReadOnlyList<ObjectEntity> issues,
            IReadOnlyList<ObjectTableColumnState> layout,
            IRequest request
        )
        {
            var orderBy = request?.GetParameter("o")?.Value;
            var descending = string.Equals(request?.GetParameter("d")?.Value, "desc", StringComparison.OrdinalIgnoreCase);

            var column = string.IsNullOrWhiteSpace(orderBy)
                ? null
                : layout.FirstOrDefault(x => string.Equals(x.Column.Id, orderBy, StringComparison.OrdinalIgnoreCase))?.Column;

            if (column is null)
            {
                return issues.OrderByDescending(x => x.Updated);
            }

            // sorting reads the cell content, so the values of every issue in scope are
            // needed rather than only those of the page
            var projection = ObjectTableProjection.Build(issues);

            return descending
                ? issues.OrderByDescending(x => column.Read(x, projection), StringComparer.OrdinalIgnoreCase)
                : issues.OrderBy(x => column.Read(x, projection), StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Fetches the issue-kind objects of the supplied workspace. Returns an empty
        /// list when the workspace is unknown.
        /// </summary>
        /// <param name="workspaceId">The id of the workspace, or <see langword="null"/>.</param>
        /// <returns>The workspace's issues. The list may be empty.</returns>
        private static IReadOnlyList<ObjectEntity> GetIssues(Guid? workspaceId)
        {
            if (workspaceId is null)
            {
                return [];
            }

            var query = new Query<ObjectEntity>()
                .WhereEquals(x => x.WorkspaceId, workspaceId.Value)
                .WhereEquals(x => x.Kind, Model.Entities.ObjectKind.Issue);

            return [.. CoreHub.ObjectManager.GetObjects(query)];
        }

        /// <summary>
        /// Projects a single issue to a table row: one cell per column of the effective
        /// layout — including the hidden ones, because the client keeps their content and
        /// shows it the moment the column is switched on — the object endpoint an inline
        /// edit writes through, the link to the detail page, and the row menu.
        /// </summary>
        /// <param name="issue">The issue to project.</param>
        /// <param name="layout">The effective columns.</param>
        /// <param name="projection">The loaded class definitions and field values.</param>
        /// <param name="starred">Whether the calling identity has starred the issue.</param>
        /// <param name="request">The request used to resolve localized content and URIs.</param>
        /// <returns>The table row.</returns>
        private static RestApiTableRow BuildRow
        (
            ObjectEntity issue,
            IReadOnlyList<ObjectTableColumnState> layout,
            ObjectTableProjection projection,
            bool starred,
            IRequest request
        )
        {
            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>()?
                .BindParameters(new ObjectKeyParameter(issue.Key));

            return new RestApiTableRow()
            {
                Id = issue.Id.ToString(),
                Cells = [.. layout.Select(x => new RestApiTableCell()
                {
                    Content = x.Column.Read?.Invoke(issue, projection)
                })],
                Options = GetOptions(issue, starred, request).Select(o => o.ToJson()),
                Bind = BuildRowBinding(issue, layout, projection),
                Uri = uri?.ToString(),
                RestApi = ResolveObjectRestUri(issue, request)?.ToString(),
                // a starred issue is marked rather than given a column of its own; the
                // star sits beside the object icon in the row's leading cell
                Icon = starred ? "fas fa-star" : null,
                Image = issue.Icon?.Uri?.ToString()
            };
        }

        /// <summary>
        /// Names the columns this row cannot be edited in, so the cell renderer offers no
        /// editor there.
        /// </summary>
        /// <remarks>
        /// A field column folds the same-named fields of every issue class of the
        /// workspace, but a class-specific field — the planned dates of a change, the
        /// impact of an incident — exists on one class only. An object of another class
        /// has nowhere to put such a value, and an edit of it would be dropped on save and
        /// silently revert on the next query. The row therefore reports those columns, by
        /// the payload name their editor would write, and the renderer draws them
        /// read-only.
        /// </remarks>
        /// <param name="issue">The issue the row shows.</param>
        /// <param name="layout">The effective columns.</param>
        /// <param name="projection">The loaded class definitions and field values.</param>
        /// <returns>The row binding payload.</returns>
        private static IDictionary<string, object> BuildRowBinding
        (
            ObjectEntity issue,
            IReadOnlyList<ObjectTableColumnState> layout,
            ObjectTableProjection projection
        )
        {
            var blocked = layout
                .Select(x => x.Column)
                .Where(x => x.FieldIds.Count > 0 && !string.IsNullOrEmpty(x.Name))
                .Where(x => !projection.DefinesField(issue, x.FieldIds))
                .Select(x => x.Name);

            return new Dictionary<string, object>
            {
                ["readonly"] = string.Join(",", blocked)
            };
        }

        /// <summary>
        /// Returns the object CRUD endpoint addressed at the supplied issue, which is
        /// what an inline cell edit PUTs its <c>{ name: value }</c> payload to.
        /// </summary>
        /// <param name="issue">The issue the row shows.</param>
        /// <param name="request">The incoming request.</param>
        /// <returns>The bound endpoint address.</returns>
        private static IUri ResolveObjectRestUri(ObjectEntity issue, IRequest request)
        {
            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Objects.Index>();

            return uri?
                .Add(new UriQuery("id", issue.Id.ToString()))
                .BindParameters(request);
        }

        /// <summary>
        /// Builds the row overflow menu: edit and clone modals, the star toggle (whose
        /// label reflects the current state and whose link flips it), and the delete
        /// modal.
        /// </summary>
        /// <param name="issue">The issue the options act on. Cannot be null.</param>
        /// <param name="starred">Whether the calling identity has starred the issue.</param>
        /// <param name="request">The request used to resolve localized labels and URIs.</param>
        /// <returns>The overflow menu options.</returns>
        private static IEnumerable<RestApiOption> GetOptions(ObjectEntity issue, bool starred, IRequest request)
        {
            var keyParameter = new ObjectKeyParameter(issue.Key);
            var editUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Issue._objectkey_.Edit>()?
                .BindParameters(request)
                .BindParameters(keyParameter);
            var cloneUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Issue._objectkey_.Clone>()?
                .BindParameters(request)
                .BindParameters(keyParameter);
            var deleteUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Issue._objectkey_.Delete>()?
                .BindParameters(request)
                .BindParameters(keyParameter);
            var favoriteUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Issue._objectkey_.Favorite>()?
                .BindParameters(request)
                .BindParameters(keyParameter);

            var iconTheme = request?.ApplicationContext?.DefaultTheme?.IconTheme ?? WebExpress.WebCore.WebIcon.TypeIconTheme.Light;

            yield return new RestApiOptionHeader(request)
            {
                Text = "webexpress.webapp:header.setting.label"
            };

            yield return new RestApiOptionEdit(request)
            {
                PrimaryAction = new ActionModal("modal-form", editUri, TypeModalSize.ExtraLarge)
            };

            yield return new RestApiOptionClone(request)
            {
                PrimaryAction = new ActionModal("modal-form", cloneUri, TypeModalSize.ExtraLarge)
            };

            // toggle the calling identity's star; the label reflects the current state
            // and the link flips it, redirecting back to the object detail page
            yield return new RestApiOptionCustom(request)
            {
                Text = I18N.Translate(request, starred
                    ? "kleenestar.core:object.favorite.remove.label"
                    : "kleenestar.core:object.favorite.add.label"),
                Icon = new WebExpress.WebUI.WebIcon.IconStar(iconTheme),
                Uri = favoriteUri
            };

            yield return new RestApiOptionSeparator(request);
            yield return new RestApiOptionDelete(request)
            {
                PrimaryAction = new ActionModal("modal-form", deleteUri, TypeModalSize.Small)
            };
        }

        /// <summary>
        /// Reads the column configuration payload of a <c>PUT</c>.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <returns>The payload, or <see langword="null"/> when the body carries none.</returns>
        private static RestApiTableConfigurePayload ReadConfigurePayload(IRequest request)
        {
            if (request is not Request raw ||
                raw.Content is not { Length: > 0 } content ||
                raw.Header?.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) != true)
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<RestApiTableConfigurePayload>(content);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// Parses an integer request parameter, falling back to a default when the
        /// parameter is missing or not a number.
        /// </summary>
        /// <param name="request">The request carrying the parameter.</param>
        /// <param name="name">The parameter name.</param>
        /// <param name="fallback">The value returned when parsing fails.</param>
        /// <returns>The parsed value, or <paramref name="fallback"/>.</returns>
        private static int ParseInt(IRequest request, string name, int fallback)
        {
            var raw = request?.GetParameter(name)?.Value;

            return int.TryParse(raw, out var value) ? value : fallback;
        }
    }
}
