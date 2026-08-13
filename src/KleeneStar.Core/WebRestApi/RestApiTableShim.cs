using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WebRestApi
{
    /// <summary>
    /// Adds the two things a REST table's client renders but its server-side model cannot express:
    /// hierarchical rows and cells whose content is markup.
    /// </summary>
    /// <remarks>
    /// The table component supports both. <c>webexpress.webapp.tableModel.normalizeRows</c> recurses
    /// into a row's <c>children</c> and honours its <c>expanded</c> flag, and the table draws the
    /// expand toggles and the indentation; <c>webexpress.webui.table.js</c> parses a cell whose
    /// <c>html</c> flag is set into nodes instead of writing it as text. Neither member exists on
    /// <c>RestApiTableRow</c> or <c>RestApiTableCell</c>, and a derived type does not help: the
    /// result serializes the declared element types, so System.Text.Json drops whatever a subclass
    /// adds. Reshaping the serialized payload is therefore the one place the members can be added
    /// without reimplementing the endpoint's filtering, sorting and paging. Remove this shim once
    /// the framework's row and cell models carry the members themselves.
    /// </remarks>
    public static class RestApiTableShim
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// Applies the shim to a serialized table response.
        /// </summary>
        /// <param name="response">The response produced by the table endpoint.</param>
        /// <param name="parents">
        /// Maps a row id to the id of its parent row, which the rows are nested along. A row whose
        /// parent is not part of the response — because it lives on another page, was filtered out,
        /// or has no parent at all — becomes a root of its own, so the tree always shows every row
        /// the query returned. Pass null to leave the rows flat.
        /// </param>
        /// <param name="htmlColumns">
        /// The ids of the columns whose cells carry markup rather than text. Pass null when every
        /// cell is text.
        /// </param>
        /// <returns>
        /// The same response with its rows reshaped, or unchanged when its content is not the
        /// expected json document.
        /// </returns>
        public static IResponse Apply(IResponse response, IReadOnlyDictionary<string, string> parents = null, IReadOnlyCollection<string> htmlColumns = null)
        {
            var document = Parse(response);

            if (document is null || document["rows"] is not JsonArray rows || rows.Count == 0)
            {
                return response;
            }

            MarkHtmlCells(rows, ResolveColumnIndexes(document, htmlColumns));

            var nested = Nest(rows, parents);

            if (nested is not null)
            {
                document["rows"] = nested;
            }

            response.Content = Encoding.UTF8.GetBytes(document.ToJsonString(_options));

            return response;
        }

        /// <summary>
        /// Reads the json document a response carries.
        /// </summary>
        /// <param name="response">The response to read.</param>
        /// <returns>The parsed document, or null when the content is not json.</returns>
        private static JsonNode Parse(IResponse response)
        {
            var json = response?.Content switch
            {
                byte[] bytes => Encoding.UTF8.GetString(bytes),
                string text => text,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonNode.Parse(json);
            }
            catch (JsonException)
            {
                // not the document this shim knows; leaving it untouched keeps the endpoint
                // working, only without the reshaping
                return null;
            }
        }

        /// <summary>
        /// Resolves the positions the named columns occupy in the response.
        /// </summary>
        /// <remarks>
        /// A cell is matched to its column by position, which is how the client maps them too, so
        /// the index is taken from the response's own column list rather than from the endpoint's
        /// defaults — a user who reordered the table then still gets the right cells marked.
        /// </remarks>
        /// <param name="document">The table document.</param>
        /// <param name="columnIds">The ids to look up.</param>
        /// <returns>The zero-based positions, which may be empty.</returns>
        private static IReadOnlySet<int> ResolveColumnIndexes(JsonNode document, IReadOnlyCollection<string> columnIds)
        {
            if (columnIds is null || columnIds.Count == 0 || document["columns"] is not JsonArray columns)
            {
                return new HashSet<int>();
            }

            return columns
                .Select((column, index) => (Id: column?["id"]?.GetValue<string>(), Index: index))
                .Where(x => x.Id is not null && columnIds.Contains(x.Id, StringComparer.OrdinalIgnoreCase))
                .Select(x => x.Index)
                .ToHashSet();
        }

        /// <summary>
        /// Flags the cells at the given positions as markup.
        /// </summary>
        /// <param name="rows">The rows to walk.</param>
        /// <param name="indexes">The positions of the markup-bearing cells.</param>
        private static void MarkHtmlCells(JsonArray rows, IReadOnlySet<int> indexes)
        {
            if (indexes.Count == 0)
            {
                return;
            }

            foreach (var row in rows)
            {
                if (row?["cells"] is not JsonArray cells)
                {
                    continue;
                }

                for (var index = 0; index < cells.Count; index++)
                {
                    // an empty cell stays text, so the client does not build a document
                    // fragment for nothing
                    if (indexes.Contains(index) && !string.IsNullOrEmpty(cells[index]?["content"]?.GetValue<string>()))
                    {
                        cells[index]["html"] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Nests the rows below their parents.
        /// </summary>
        /// <param name="rows">The flat rows of the response.</param>
        /// <param name="parents">The row-to-parent map, or null to leave the rows flat.</param>
        /// <returns>The nested rows, or null when there is nothing to nest.</returns>
        private static JsonArray Nest(JsonArray rows, IReadOnlyDictionary<string, string> parents)
        {
            if (parents is null || parents.Count == 0)
            {
                return null;
            }

            var order = new List<string>();
            var byId = new Dictionary<string, JsonNode>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                var id = row?["id"]?.GetValue<string>();

                if (!string.IsNullOrEmpty(id) && byId.TryAdd(id, row))
                {
                    order.Add(id);
                }
            }

            string ParentOf(string id)
            {
                var parentId = parents.TryGetValue(id, out var candidate) ? candidate : null;

                // a parent outside the response, or a row pointing at itself, cannot nest
                return parentId is not null
                    && !string.Equals(parentId, id, StringComparison.OrdinalIgnoreCase)
                    && byId.ContainsKey(parentId)
                        ? parentId
                        : null;
            }

            var childrenOf = order
                .Where(id => ParentOf(id) is not null)
                .GroupBy(ParentOf, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            var nested = new JsonArray();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var id in order.Where(id => ParentOf(id) is null))
            {
                nested.Add(Build(id, byId, childrenOf, visited));
            }

            // a parent map that closes a cycle would leave its members unreachable from any root;
            // they are appended flat so no row of the response is lost
            foreach (var id in order.Where(id => !visited.Contains(id)))
            {
                nested.Add(Build(id, byId, childrenOf, visited));
            }

            return nested;
        }

        /// <summary>
        /// Produces a detached copy of a row with its descendants nested below it.
        /// </summary>
        /// <param name="id">The id of the row to copy.</param>
        /// <param name="byId">The original row nodes by id.</param>
        /// <param name="childrenOf">The parent-id to child-ids lookup, in response order.</param>
        /// <param name="visited">
        /// The ids already placed in the tree; it terminates the recursion should the parent map
        /// contain a cycle.
        /// </param>
        /// <returns>The copied row carrying its subtree.</returns>
        private static JsonNode Build(string id, IReadOnlyDictionary<string, JsonNode> byId, IReadOnlyDictionary<string, List<string>> childrenOf, ISet<string> visited)
        {
            var copy = byId[id].DeepClone();

            if (!visited.Add(id) || !childrenOf.TryGetValue(id, out var children))
            {
                return copy;
            }

            var array = new JsonArray();

            foreach (var child in children.Where(x => !visited.Contains(x)))
            {
                array.Add(Build(child, byId, childrenOf, visited));
            }

            if (array.Count == 0)
            {
                return copy;
            }

            copy["children"] = array;
            copy["expanded"] = true;

            return copy;
        }
    }
}
