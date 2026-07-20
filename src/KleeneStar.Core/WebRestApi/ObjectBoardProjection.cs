using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebRestApi
{
    /// <summary>
    /// Captures the per-class lookup data needed to project workspace objects onto the
    /// Kanban and Scrum boards — the workflow and priority fields of the class and its
    /// active statuses.
    /// </summary>
    internal sealed class ObjectBoardClassContext
    {
        /// <summary>Gets the class the context belongs to.</summary>
        public Class Class { get; init; }

        /// <summary>Gets the workflow-typed field of the class, or <see langword="null"/>.</summary>
        public Field WorkflowField { get; init; }

        /// <summary>Gets the priority-typed field of the class, or <see langword="null"/>.</summary>
        public Field PriorityField { get; init; }

        /// <summary>Gets the active statuses of the class.</summary>
        public IReadOnlyList<Status> Statuses { get; init; }
    }

    /// <summary>
    /// Provides the shared projection logic of the object board REST endpoints (Kanban,
    /// Scrum backlog, Scrum sprint): resolving an object's workflow-field value to a
    /// status category, its priority value to a display code, and deriving assignee
    /// display data. Mirrors the resolution rules of the portal's issue projection.
    /// </summary>
    internal static class ObjectBoardProjection
    {
        /// <summary>
        /// Builds the board context of a class from the field, status and category
        /// managers.
        /// </summary>
        /// <param name="cls">The class to capture.</param>
        /// <returns>The board projection context of the class.</returns>
        public static ObjectBoardClassContext BuildClassContext(Class cls)
        {
            var fields = CoreHub.FieldManager
                .GetFields(new ClassIdParameter(cls.Id))
                .Where(f => !f.Deprecated && f.State == FieldState.Active)
                .ToList();

            var statuses = CoreHub.StatusManager
                .GetStatuses(new ClassIdParameter(cls.Id))
                .Where(s => s.State == StatusState.Active)
                .ToList();

            return new ObjectBoardClassContext
            {
                Class = cls,
                WorkflowField = fields.FirstOrDefault(f => f.FieldType == FieldType.Workflow),
                PriorityField = fields.FirstOrDefault(f => f.FieldType == FieldType.Priority),
                Statuses = statuses
            };
        }

        /// <summary>
        /// Returns all status categories ordered for board display: To Do, In Progress,
        /// Waiting, Done, then any further categories alphabetically.
        /// </summary>
        /// <returns>The ordered categories.</returns>
        public static IReadOnlyList<StatusCategory> GetOrderedCategories()
        {
            return [.. CoreHub.StatusManager
                .GetStatusCategories(new Query<StatusCategory>())
                .OrderBy(CategoryRank)
                .ThenBy(x => x.Name)];
        }

        /// <summary>
        /// Resolves the status category of an object from its workflow-field value —
        /// first via the stamped status (by normalized name, then by status id), then by
        /// a direct category-name match of the raw payload. Returns
        /// <see langword="null"/> when the object carries no resolvable value.
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <param name="context">The board context of the object's class.</param>
        /// <param name="categories">All status categories, keyed by id.</param>
        /// <returns>The resolved category, or <see langword="null"/>.</returns>
        public static StatusCategory ResolveCategory(Guid objectId, ObjectBoardClassContext context, IReadOnlyDictionary<Guid, StatusCategory> categories)
        {
            if (context?.WorkflowField is null)
            {
                return null;
            }

            var data = CoreHub.ValueManager.GetValue(objectId, context.WorkflowField.Id)?.Data;
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            var normalized = Normalize(data);

            var status = context.Statuses.FirstOrDefault(s => Normalize(s.Name) == normalized)
                ?? context.Statuses.FirstOrDefault(s => string.Equals(s.Id.ToString(), data, StringComparison.OrdinalIgnoreCase));

            if (status is not null && categories.TryGetValue(status.CategoryId, out var category))
            {
                return category;
            }

            // seeded payloads like "done" carry no matching status but name a category
            return categories.Values.FirstOrDefault(c => Normalize(c.Name) == normalized);
        }

        /// <summary>
        /// Resolves the priority display code of an object from its priority-field
        /// value: a leading <c>P&lt;digit&gt;</c> token wins, well-known severity names
        /// collapse to <c>P1</c>–<c>P4</c>, anything else is shown verbatim. Returns
        /// <see cref="string.Empty"/> when the object carries no priority.
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <param name="context">The board context of the object's class.</param>
        /// <returns>The priority display code, or an empty string.</returns>
        public static string ResolvePriorityCode(Guid objectId, ObjectBoardClassContext context)
        {
            if (context?.PriorityField is null)
            {
                return string.Empty;
            }

            var data = CoreHub.ValueManager.GetValue(objectId, context.PriorityField.Id)?.Data;
            if (string.IsNullOrWhiteSpace(data))
            {
                return string.Empty;
            }

            var trimmed = data.Trim();

            if (trimmed.Length >= 2
                && (trimmed[0] == 'P' || trimmed[0] == 'p')
                && char.IsDigit(trimmed[1])
                && (trimmed.Length == 2 || !char.IsLetterOrDigit(trimmed[2])))
            {
                return trimmed[..2].ToUpperInvariant();
            }

            return Normalize(trimmed) switch
            {
                "critical" => "P1",
                "high" => "P2",
                "medium" => "P3",
                "low" => "P4",
                _ => trimmed
            };
        }

        /// <summary>
        /// Returns the display label of a status category, splitting camel-cased names
        /// ("InProgress") into words ("In Progress").
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The display label.</returns>
        public static string CategoryLabel(StatusCategory category)
        {
            return Regex.Replace(category?.Name ?? string.Empty, "(?<=[a-z])(?=[A-Z])", " ");
        }

        /// <summary>
        /// Returns the system color CSS class of a status category for the board column
        /// header.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The CSS class.</returns>
        public static string CategoryColorCss(StatusCategory category)
        {
            return Normalize(category?.Name) switch
            {
                "todo" => "wx-color-secondary",
                "inprogress" => "wx-color-primary",
                "waiting" => "wx-color-warning",
                "done" => "wx-color-success",
                _ => "wx-color-secondary"
            };
        }

        /// <summary>
        /// Returns the collapsed Scrum item status string of a status category as the
        /// scrum controls expect it ("todo", "doing", "waiting", "done").
        /// </summary>
        /// <param name="category">The category, or <see langword="null"/>.</param>
        /// <returns>The item status string.</returns>
        public static string CategoryItemStatus(StatusCategory category)
        {
            return Normalize(category?.Name) switch
            {
                "inprogress" => "doing",
                "waiting" => "waiting",
                "done" => "done",
                _ => "todo"
            };
        }

        /// <summary>
        /// Derives the short initials shown inside an assignee avatar from the display
        /// name (first letter of the first and last word, upper-cased).
        /// </summary>
        /// <param name="name">The display name.</param>
        /// <returns>The initials, or an empty string.</returns>
        public static string Initials(string name)
        {
            var words = (name ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return words.Length switch
            {
                0 => string.Empty,
                1 => char.ToUpperInvariant(words[0][0]).ToString(),
                _ => $"{char.ToUpperInvariant(words[0][0])}{char.ToUpperInvariant(words[^1][0])}"
            };
        }

        /// <summary>
        /// Returns a deterministic avatar background color for an identity, so the same
        /// person always renders with the same hue.
        /// </summary>
        /// <param name="identityId">The identity id.</param>
        /// <returns>An HSL CSS color.</returns>
        public static string AvatarColor(Guid identityId)
        {
            var hue = Math.Abs(identityId.GetHashCode()) % 360;

            return $"hsl({hue}, 45%, 45%)";
        }

        /// <summary>
        /// Normalizes a name for comparison: keeps letters and digits only, lower-cased.
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        public static string Normalize(string value)
        {
            return new string((value ?? string.Empty).Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        }

        /// <summary>
        /// Returns the board display rank of a status category (To Do first, Done last).
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The ordering rank.</returns>
        private static int CategoryRank(StatusCategory category)
        {
            return Normalize(category?.Name) switch
            {
                "todo" => 0,
                "inprogress" => 1,
                "waiting" => 2,
                "done" => 3,
                _ => 4
            };
        }
    }
}
