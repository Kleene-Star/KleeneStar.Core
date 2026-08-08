using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Core.WebRestApi
{
    /// <summary>
    /// The cell template of an object overview table column.
    /// </summary>
    /// <remarks>
    /// The stock templates of <c>WebExpress.WebApp</c> render an editor when their
    /// <c>editable</c> option is set, but they wire it to no endpoint: the inline editor
    /// they mount announces the new value through the save event and leaves the storing
    /// to whoever listens, and the table listens to nothing. A table built from them can
    /// be edited but never saves.
    ///
    /// This template therefore names its own renderer (<c>kleenestar-cell</c>, registered
    /// by <c>assets/js/tableinlineedit.js</c>), which points the inline editor at the
    /// object endpoint the row carries in <see cref="RestApiTableRow.RestApi"/> and gives
    /// the editor the column's property name, so a finished edit is a PUT of
    /// <c>{ name: value }</c> against that object. The read-only side is delegated to the
    /// stock renderer named by <see cref="Kind"/>, so a cell looks exactly like the same
    /// cell of any other table.
    /// </remarks>
    internal sealed class ObjectTableColumnTemplate : IRestApiTableColumnTemplate
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        /// <summary>
        /// Gets the renderer id. The client registry resolves it to the KleeneStar cell
        /// renderer rather than to one of the stock ones.
        /// </summary>
        public string Type => "kleenestar-cell";

        /// <summary>
        /// Gets a value indicating whether the cell offers an inline editor.
        /// </summary>
        public bool Editable { get; private init; }

        /// <summary>
        /// Gets the stock renderer the read-only side of the cell is drawn with, one of
        /// <c>text</c>, <c>numeric</c>, <c>date</c>, <c>tag</c> or <c>combo</c>.
        /// </summary>
        public string Kind { get; private init; }

        /// <summary>
        /// Gets the selectable items of a <c>combo</c> cell, empty for the other kinds.
        /// </summary>
        public IReadOnlyList<RestApiTableColumnTemplateItem> Items { get; private init; } = [];

        /// <summary>
        /// Gets the date format of a <c>date</c> cell.
        /// </summary>
        public string Format { get; private init; } = "yyyy-MM-dd";

        /// <summary>
        /// Gets the address of the object CRUD endpoint an inline edit writes through.
        /// The renderer appends the row's object id to it.
        /// </summary>
        /// <remarks>
        /// The address belongs on the row rather than on the column, and
        /// <see cref="RestApiTableRow.RestApi"/> is where it is sent. The REST table
        /// control drops that property while normalising the response, though, so a cell
        /// renderer never sees it; carrying the address on the column as well is what
        /// makes the editor reach the endpoint at all. Remove this once the control
        /// forwards the row property.
        /// </remarks>
        public string Endpoint { get; private set; }

        /// <summary>
        /// Points the template's inline editor at the object CRUD endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint address.</param>
        public void BindEndpoint(string endpoint)
        {
            Endpoint = endpoint;
        }

        /// <summary>
        /// Returns a read-only cell drawn with the supplied stock renderer.
        /// </summary>
        /// <param name="kind">The stock renderer id.</param>
        /// <returns>The template.</returns>
        public static ObjectTableColumnTemplate ReadOnly(string kind)
        {
            return new ObjectTableColumnTemplate { Kind = kind, Editable = false };
        }

        /// <summary>
        /// Returns an inline-editable cell drawn with the supplied stock renderer.
        /// </summary>
        /// <param name="kind">The stock renderer id.</param>
        /// <returns>The template.</returns>
        public static ObjectTableColumnTemplate Input(string kind)
        {
            return new ObjectTableColumnTemplate { Kind = kind, Editable = true };
        }

        /// <summary>
        /// Returns a cell that shows one of a fixed set of values and, when editable,
        /// offers that set as a drop-down.
        /// </summary>
        /// <param name="items">The selectable items.</param>
        /// <param name="editable">Whether the cell offers the editor.</param>
        /// <returns>The template.</returns>
        public static ObjectTableColumnTemplate Combo(IReadOnlyList<RestApiTableColumnTemplateItem> items, bool editable)
        {
            return new ObjectTableColumnTemplate
            {
                Kind = "combo",
                Editable = editable,
                Items = items ?? []
            };
        }

        /// <summary>
        /// Serializes the template into the <c>{ type, options }</c> shape the client
        /// template registry consumes. The items travel as an embedded JSON string,
        /// which is the shape the stock combo renderer parses, so the options can be
        /// handed to it unchanged.
        /// </summary>
        /// <returns>The JSON representation.</returns>
        public string ToJson()
        {
            var options = new Dictionary<string, object>
            {
                ["kind"] = Kind,
                ["editable"] = Editable,
                ["format"] = Format,
                ["endpoint"] = Endpoint
            };

            if (Items.Count > 0)
            {
                options["options"] = JsonSerializer.Serialize(Items.Select(x => new
                {
                    value = x.Id,
                    text = x.Text
                }), _jsonOptions);
            }

            return JsonSerializer.Serialize(new
            {
                type = Type,
                options
            }, _jsonOptions);
        }
    }
}
