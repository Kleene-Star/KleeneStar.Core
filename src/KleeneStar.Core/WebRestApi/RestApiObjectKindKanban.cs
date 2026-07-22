using KleeneStar.Core.WebParameter;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebRestApi
{
    /// <summary>
    /// Project-wide base for the object Kanban endpoint of a kind's overview tab control.
    /// Columns are the workflow status categories (To Do, In Progress, Waiting, Done),
    /// swimlanes are the classes of the workspace, and each active object of the
    /// <see cref="Kind"/> becomes a card placed by its workflow-field value. A concrete
    /// subclass only fixes the kind it lists (issue, asset, …); each concrete endpoint
    /// registers at its own route, so the base must stay abstract.
    /// </summary>
    public abstract class RestApiObjectKindKanban : RestApiKanban<Model.Entities.Object>
    {
        /// <summary>
        /// Gets the persisted kind key the board is scoped to.
        /// </summary>
        protected abstract string Kind { get; }

        /// <summary>
        /// Resolves an optional sprint the board is additionally scoped to: when non-null,
        /// only objects committed to that sprint become cards (and only their classes form
        /// swimlanes). The default returns <see langword="null"/>, so the board shows every
        /// object of the kind in the workspace. The sprint board of the Scrum tab overrides
        /// this to show the active sprint only.
        /// </summary>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The sprint id to scope to, or <see langword="null"/> for the whole workspace.</returns>
        protected virtual Guid? ResolveSprint(IRequest request) => null;

        /// <summary>
        /// Applies an optional in-memory quickfilter to the board's objects (both the cards
        /// and the swimlane population). The default is a no-op, so the board shows every
        /// object of the kind/sprint. The sprint board overrides this to honour the personal
        /// scope chips (assigned to me, starred) that a WebIndex query cannot express.
        /// </summary>
        /// <param name="objects">The objects that would become cards / populate swimlanes.</param>
        /// <param name="request">The request that provides the operational context.</param>
        /// <returns>The filtered objects.</returns>
        protected virtual IEnumerable<Model.Entities.Object> ApplyQuickfilter(IEnumerable<Model.Entities.Object> objects, IRequest request) => objects;

        /// <summary>
        /// Returns a <see cref="KleeneStarDbContext"/> so <see cref="CoreHub.ObjectManager"/>
        /// can run its queries; the base class' default query context would cast to null
        /// in the manager and trigger an NRE downstream.
        /// </summary>
        protected override IQueryContext CreateContext()
        {
            return ModelHub.CreateDbContext();
        }

        /// <summary>
        /// Returns one kanban column per workflow status category, ordered To Do,
        /// In Progress, Waiting, Done.
        /// </summary>
        protected override IEnumerable<RestApiKanbanColumn> RetrieveColumns(IRequest request)
        {
            return ObjectBoardProjection.GetOrderedCategories()
                .Select(category => new RestApiKanbanColumn
                {
                    Id = category.Id.ToString(),
                    Label = ObjectBoardProjection.CategoryLabel(category),
                    ColorCss = ObjectBoardProjection.CategoryColorCss(category)
                });
        }

        /// <summary>
        /// Returns one swimlane per class that has at least one active object of the kind
        /// in the workspace, ordered by class name.
        /// </summary>
        protected override IEnumerable<RestApiKanbanSwimlane> RetrieveSwimlanes(IRequest request)
        {
            var workspace = GetWorkspace(request);

            if (workspace is null)
            {
                yield break;
            }

            var populatedClassIds = GetActiveObjects(workspace.Id, ResolveSprint(request), request)
                .Select(x => x.ClassId)
                .ToHashSet();

            var classes = CoreHub.ClassManager
                .GetClasses(new Query<Model.Entities.Class>().WhereEquals(x => x.WorkspaceId, workspace.Id))
                .Where(x => populatedClassIds.Contains(x.Id))
                .OrderBy(x => x.Name);

            foreach (var cls in classes)
            {
                yield return new RestApiKanbanSwimlane
                {
                    Id = cls.Id.ToString(),
                    Label = cls.Name,
                    Expanded = true
                };
            }
        }

        /// <summary>
        /// Returns one card per active object of the kind, placed in the column of its
        /// workflow status category and the swimlane of its class. Objects without a
        /// resolvable workflow value fall into the first (To Do) column.
        /// </summary>
        protected override IEnumerable<RestApiKanbanCard> RetrieveCards(IQuery<Model.Entities.Object> query, IQueryContext context, IRequest request)
        {
            var workspace = GetWorkspace(request);

            if (workspace is null)
            {
                yield break;
            }

            var categories = ObjectBoardProjection.GetOrderedCategories();
            var categoriesById = categories.ToDictionary(x => x.Id, x => x);
            var defaultCategory = categories.FirstOrDefault();
            var contextByClass = new Dictionary<Guid, ObjectBoardClassContext>();
            var identityById = new Dictionary<Guid, Identity>();
            var sprintId = ResolveSprint(request);

            query = query
                .WhereEquals(x => x.WorkspaceId, workspace.Id)
                .WhereEquals(x => x.Kind, Kind);

            var cards = CoreHub.ObjectManager.GetObjects(query, context)
                .Where(x => x.State == WorkspaceState.Active)
                .Where(x => sprintId is null || x.SprintId == sprintId);
            cards = ApplyQuickfilter(cards, request);

            foreach (var entity in cards)
            {
                if (!contextByClass.TryGetValue(entity.ClassId, out var classContext))
                {
                    var cls = CoreHub.ClassManager.GetClass(entity.ClassId);
                    classContext = cls is null ? null : ObjectBoardProjection.BuildClassContext(cls);
                    contextByClass[entity.ClassId] = classContext;
                }

                var category = ObjectBoardProjection.ResolveCategory(entity.Id, classContext, categoriesById)
                    ?? defaultCategory;

                yield return BuildCard(entity, classContext, category, identityById);
            }
        }

        /// <summary>
        /// Builds the kanban card of a single object, including the assignee avatar data
        /// and the priority/story-point footer chips.
        /// </summary>
        /// <param name="entity">The object to project.</param>
        /// <param name="classContext">The board context of the object's class.</param>
        /// <param name="category">The resolved status category of the object.</param>
        /// <param name="identityById">A request-scoped identity cache.</param>
        /// <returns>The kanban card.</returns>
        private static RestApiKanbanCard BuildCard
        (
            Model.Entities.Object entity,
            ObjectBoardClassContext classContext,
            StatusCategory category,
            Dictionary<Guid, Identity> identityById
        )
        {
            var assignee = ResolveIdentity(entity.AssigneeId, identityById);

            var card = new RestApiKanbanCard
            {
                Id = entity.Id.ToString(),
                Label = string.IsNullOrWhiteSpace(entity.Summary) ? entity.Key : entity.Summary,
                Html = $"<strong>{WebUtility.HtmlEncode(entity.Key)}</strong><br/>{WebUtility.HtmlEncode(entity.Summary)}",
                ColumnId = category?.Id.ToString(),
                SwimlaneId = entity.ClassId.ToString(),
                AssigneeId = assignee?.Id.ToString(),
                AssigneeName = assignee?.Name,
                AssigneeInitials = assignee is null ? null : ObjectBoardProjection.Initials(assignee.Name),
                AssigneeColor = assignee is null ? null : ObjectBoardProjection.AvatarColor(assignee.Id),
                Footer = BuildFooter(entity, classContext).ToList()
            };

            return card;
        }

        /// <summary>
        /// Builds the footer chips of a card: the priority code (when the object carries
        /// a priority value) and the story-point estimate (when estimated).
        /// </summary>
        /// <param name="entity">The object to project.</param>
        /// <param name="classContext">The board context of the object's class.</param>
        /// <returns>The footer chips.</returns>
        private static IEnumerable<RestApiKanbanCardChip> BuildFooter(Model.Entities.Object entity, ObjectBoardClassContext classContext)
        {
            var priority = ObjectBoardProjection.ResolvePriorityCode(entity.Id, classContext);

            if (!string.IsNullOrWhiteSpace(priority))
            {
                yield return new RestApiKanbanCardChip
                {
                    Label = priority,
                    Icon = new IconFlag(),
                    Color = new PropertyColorBackgroundBadge(PriorityBadgeColor(priority)),
                    Title = "Priority"
                };
            }

            if (entity.StoryPoints is int points)
            {
                yield return new RestApiKanbanCardChip
                {
                    Label = points.ToString(),
                    Icon = new IconScaleBalanced(),
                    Color = new PropertyColorBackgroundBadge(TypeColorBackgroundBadge.Secondary),
                    Title = "Story points"
                };
            }
        }

        /// <summary>
        /// Maps a priority display code to the badge color of its chip.
        /// </summary>
        /// <param name="priority">The priority display code.</param>
        /// <returns>The badge color.</returns>
        private static TypeColorBackgroundBadge PriorityBadgeColor(string priority)
        {
            return priority switch
            {
                "P1" => TypeColorBackgroundBadge.Danger,
                "P2" => TypeColorBackgroundBadge.Warning,
                "P3" => TypeColorBackgroundBadge.Info,
                "P4" => TypeColorBackgroundBadge.Secondary,
                _ => TypeColorBackgroundBadge.Secondary
            };
        }

        /// <summary>
        /// Resolves the workspace addressed by the request route.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The workspace, or <see langword="null"/>.</returns>
        private static Workspace GetWorkspace(IRequest request)
        {
            var workspaceKey = request?.GetParameter<WorkspaceKeyParameter>()?.Value;

            return CoreHub.WorkspaceManager.GetWorkspaceByKey(workspaceKey);
        }

        /// <summary>
        /// Returns the active objects of the kind in the workspace, optionally narrowed to
        /// the objects committed to <paramref name="sprintId"/>.
        /// </summary>
        /// <param name="workspaceId">The workspace id.</param>
        /// <param name="sprintId">The sprint to scope to, or <see langword="null"/> for the whole workspace.</param>
        /// <param name="request">The request that provides the operational context for the quickfilter.</param>
        /// <returns>The active objects.</returns>
        private IEnumerable<Model.Entities.Object> GetActiveObjects(Guid workspaceId, Guid? sprintId, IRequest request)
        {
            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.WorkspaceId, workspaceId)
                .WhereEquals(x => x.Kind, Kind);

            var objects = CoreHub.ObjectManager.GetObjects(query)
                .Where(x => x.State == WorkspaceState.Active)
                .Where(x => sprintId is null || x.SprintId == sprintId);

            return ApplyQuickfilter(objects, request);
        }

        /// <summary>
        /// Resolves an identity through a request-scoped cache.
        /// </summary>
        /// <param name="identityId">The identity id, or <see langword="null"/>.</param>
        /// <param name="identityById">The cache.</param>
        /// <returns>The identity, or <see langword="null"/>.</returns>
        private static Identity ResolveIdentity(Guid? identityId, Dictionary<Guid, Identity> identityById)
        {
            if (identityId is not Guid id)
            {
                return null;
            }

            if (!identityById.TryGetValue(id, out var identity))
            {
                identity = CoreHub.IdentityManager.GetIdentity(id);
                identityById[id] = identity;
            }

            return identity;
        }
    }
}
