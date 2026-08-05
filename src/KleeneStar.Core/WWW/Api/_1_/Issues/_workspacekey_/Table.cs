using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebQuickfilter;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;
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
    /// addressed workspace, most recently updated first, filtered by the search term
    /// and the personal quickfilters (starred, assigned to me, created by me,
    /// archived), and paged.
    /// </summary>
    /// <remarks>
    /// The endpoint is a plain <see cref="IRestApi"/> rather than a
    /// <c>RestApiTable&lt;Object&gt;</c> because the starred scope is a per-identity
    /// projection (the favorite flag lives on the caller's visit rows) that a WebIndex
    /// query cannot express — the base class would page the wrong set. Filtering and
    /// paging therefore run in memory over the workspace's issues. The endpoint honours
    /// the same query parameters the other REST tables use: <c>q</c> (substring search),
    /// <c>f</c> (comma-separated quickfilter ids), <c>p</c> (zero-based page number),
    /// and <c>l</c> (page size).
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
        /// Initializes a new instance of the class.
        /// </summary>
        public Table()
        {
        }

        /// <summary>
        /// Builds the filtered, paged page of the workspace's issues and returns it as
        /// a table result response.
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

            var filtered = issues
                .OrderByDescending(x => x.Updated)
                .ToList();

            var rows = filtered
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Select(x => BuildRow(x, starredIds.Contains(x.Id), request))
                .ToList();

            var result = new RestApiTableResult()
            {
                Title = null,
                Columns = BuildColumns(request),
                Rows = rows,
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
        /// Fetches the issue-kind objects of the supplied workspace, most recently
        /// updated first. Returns an empty list when the workspace is unknown.
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
        /// Builds the (fixed) column definitions of the issue table. The labels are
        /// translated manually — REST table labels are not auto-translated.
        /// </summary>
        /// <param name="request">The request used to resolve the localized labels.</param>
        /// <returns>The column definitions.</returns>
        private static IEnumerable<RestApiTableColumn> BuildColumns(IRequest request)
        {
            yield return new RestApiTableColumn()
            {
                Id = "starred",
                Label = "★",
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "key",
                Label = I18N.Translate(request, "kleenestar.core:object.kind.issues.column.key"),
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "summary",
                Label = I18N.Translate(request, "kleenestar.core:object.kind.issues.column.summary"),
                Visible = true
            };

            yield return new RestApiTableColumn()
            {
                Id = "updated",
                Label = I18N.Translate(request, "kleenestar.core:object.kind.issues.column.updated"),
                Visible = true
            };
        }

        /// <summary>
        /// Projects a single issue to a table row, linking the row to the object detail
        /// page and offering the edit, clone, star-toggle, and delete options.
        /// </summary>
        /// <param name="issue">The issue to project.</param>
        /// <param name="starred">Whether the calling identity has starred the issue.</param>
        /// <param name="request">The request used to resolve localized content and URIs.</param>
        /// <returns>The table row.</returns>
        private static RestApiTableRow BuildRow(ObjectEntity issue, bool starred, IRequest request)
        {
            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>()?
                .BindParameters(new ObjectKeyParameter(issue.Key));

            return new RestApiTableRow()
            {
                Id = issue.Id.ToString(),
                Cells =
                [
                    new RestApiTableCell() { Content = starred ? "★" : string.Empty },
                    new RestApiTableCell() { Content = issue.Key },
                    new RestApiTableCell() { Content = issue.Summary },
                    new RestApiTableCell() { Content = issue.Updated.ToString("yyyy-MM-dd HH:mm") }
                ],
                Options = GetOptions(issue, starred, request).Select(o => o.ToJson()),
                Uri = uri?.ToString(),
                Image = issue.Icon?.Uri?.ToString()
            };
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
