/**
 * KleeneStar empty-state for REST-backed tables.
 *
 * A ControlDataTable renders its rows on the client, so the server cannot know at render
 * time whether a view is empty, and the control itself offers no placeholder: an empty
 * result simply paints a table with a header and nothing under it. This attaches the
 * missing placeholder without touching the control.
 *
 * The message is authored and translated on the server and rendered next to the table as
 *
 *     <div class="ks-table-empty" data-ks-empty-for="TABLE_ID" hidden>…</div>
 *
 * so it participates in the page's normal styling and localization. This file only decides
 * when it is shown.
 *
 * The trigger is the table's own "webexpress.webui.data.arrived" event, which the data
 * service dispatches after every load and which bubbles to the document — so one listener
 * serves every table on the page, and the placeholder is re-evaluated on the first paint,
 * on a search, on a filter chip and on every page step alike. Listening on the document
 * also means a table that is instantiated later (a tab pane cloned from a template) is
 * covered without re-registering anything.
 */
(function () {
    /**
     * The attribute an empty-state element carries, naming the table it belongs to.
     */
    const TARGET_ATTRIBUTE = "data-ks-empty-for";

    /**
     * The event the REST table dispatches once a response has been integrated.
     */
    const DATA_ARRIVED = "webexpress.webui.data.arrived";

    /**
     * Reads the number of rows a table response carries.
     *
     * The row array is authoritative, because it is what the table actually painted. The
     * pagination total is only consulted when the response carries no array at all, which
     * is what an endpoint answering an error shape looks like — treating that as "no rows"
     * would flash the placeholder over a table that is merely broken.
     *
     * @param {object} response The response the table reported.
     * @returns {number|null} The row count, or null when it cannot be determined.
     */
    const countRows = (response) => {
        if (!response) {
            return null;
        }

        if (Array.isArray(response.rows)) {
            return response.rows.length;
        }

        const total = response.pagination && response.pagination.totalCount;

        return typeof total === "number" ? total : null;
    };

    /**
     * Resolves the empty-state element belonging to a table.
     *
     * @param {string} id The id of the table host element.
     * @returns {HTMLElement|null} The placeholder, or null when the table declares none.
     */
    const findPlaceholder = (id) => {
        if (!id) {
            return null;
        }

        return document.querySelector("[" + TARGET_ATTRIBUTE + "=\"" + CSS.escape(id) + "\"]");
    };

    document.addEventListener(DATA_ARRIVED, (event) => {
        const detail = event.detail || {};
        const id = detail.id || (detail.sender && detail.sender.id);
        const placeholder = findPlaceholder(id);

        if (!placeholder) {
            return;
        }

        const rows = countRows(detail.response);

        if (rows === null) {
            return;
        }

        placeholder.hidden = rows > 0;

        // the table keeps its header when it has no rows, which reads as a column layout
        // waiting for data rather than as an answer; hiding it puts the message alone on
        // the empty view and brings the layout straight back when rows return
        const table = document.getElementById(id);

        if (table) {
            table.hidden = rows === 0;
        }
    });
})();
