/**
 * KleeneStar inline-editable table cell renderer.
 *
 * Registers the "kleenestar-cell" template in webexpress.webui.TableTemplates, which the
 * object overview tables name for every one of their columns. A cell is drawn read-only
 * by the stock renderer named in its "kind" option, so it looks exactly like the same
 * cell of any other table, and an editable one wraps that in a
 * webexpress.webui.SmartEditCtrl: hovering shows a pencil, a click or a double click
 * swaps the value for an editor, and confirming writes the new value to the object.
 *
 * The writing is done here rather than by the SmartEditCtrl, which can submit on its own
 * once it is given a "data-form-action". Three things stand in the way of that:
 *
 *   - it submits multipart form data, and the WebExpress request parser cuts such a value
 *     at the first semicolon, so every tag list and every text carrying a semicolon would
 *     be silently truncated on save. A JSON body round-trips intact.
 *   - it appends a hidden field named after "data-object-name" whose value is the
 *     control's own element id rather than the edited value.
 *   - the REST table drops the row's "restApi" property while normalising the response
 *     (see webexpress.webapp.tableModel.normalizeRows), so a renderer cannot learn the
 *     endpoint from the row anyway. It travels on the column instead, in the template
 *     option "endpoint", and the row's object id is appended here.
 *
 * Without a form action the SmartEditCtrl announces the finished edit through the save
 * event and leaves the storing to whoever listens, which is what the listener at the
 * bottom of this file does.
 */
(function () {
    /**
     * The attributes an editable cell carries for its save handler. The SmartEditCtrl
     * strips its own data-form-* and data-object-* attributes from the host element, so
     * the handler cannot read them back and these are kept alongside.
     */
    const ACTION_ATTRIBUTE = "data-ks-action";
    const NAME_ATTRIBUTE = "data-ks-name";
    const KIND_ATTRIBUTE = "data-ks-kind";

    /**
     * Reads the selectable items of a combo cell from the template options.
     * @param {Object} opts The merged template options.
     * @returns {Array<{value: string, text: string}>} The items, empty when none.
     */
    const readItems = (opts) => {
        if (Array.isArray(opts.options)) {
            return opts.options;
        }

        if (typeof opts.options !== "string" || opts.options === "") {
            return [];
        }

        try {
            const parsed = JSON.parse(opts.options);
            return Array.isArray(parsed) ? parsed : [];
        } catch (e) {
            return [];
        }
    };

    /**
     * Returns the address an edit of this cell is written to: the object endpoint of the
     * column, addressed at the object of the row.
     * @param {Object} row The row data.
     * @param {Object} opts The merged template options.
     * @returns {string|null} The address, or null when the row carries no object.
     */
    const resolveAction = (row, opts) => {
        const base = opts.endpoint || (row && row.restApi) || null;

        if (!base || !row || !row.id) {
            return null;
        }

        return base + (base.indexOf("?") >= 0 ? "&" : "?") + "id=" + encodeURIComponent(row.id);
    };

    /**
     * Returns whether the row can be edited in this column. A field column folds the
     * same-named fields of several classes, and an object whose class does not define any
     * of them has nowhere to put a value; the row names those columns in its binding.
     * @param {Object} row The row data.
     * @param {string} name The payload name of the column.
     * @returns {boolean} True when the cell may offer an editor.
     */
    const isWritable = (row, name) => {
        const blocked = row && row.bind ? row.bind.readonly : null;

        if (typeof blocked !== "string" || blocked === "") {
            return true;
        }

        return blocked.split(",").indexOf(name) < 0;
    };

    /**
     * Draws the cell read-only, with the stock renderer of its kind.
     * @param {*} val The cell value.
     * @param {Object} table The table controller.
     * @param {Object} row The row data.
     * @param {Object} cell The cell data.
     * @param {string} name The payload name of the column.
     * @param {Object} opts The merged template options.
     * @returns {Node|string} The rendered content.
     */
    const renderReadOnly = (val, table, row, cell, name, opts) => {
        const stock = webexpress.webui.TableTemplates.get(opts.kind || "text");

        if (!stock) {
            const span = document.createElement("span");
            span.textContent = val === null || typeof val === "undefined" ? "" : String(val);
            return span;
        }

        return stock.fn(val, table, row, cell, name, Object.assign({}, stock.options, opts, { editable: false }));
    };

    /**
     * Builds the editor of a cell.
     * @param {*} val The cell value.
     * @param {string} name The payload name of the column.
     * @param {Object} opts The merged template options.
     * @returns {HTMLElement} The editor element.
     */
    const buildEditor = (val, name, opts) => {
        const value = val === null || typeof val === "undefined" ? "" : String(val);
        const kind = opts.kind || "text";

        if (kind === "combo") {
            const select = document.createElement("select");
            select.className = "form-select";
            select.name = name;

            readItems(opts).forEach((item) => {
                const option = document.createElement("option");
                option.value = item.value;
                option.textContent = item.text;
                option.selected = String(item.value) === value;
                select.appendChild(option);
            });

            return select;
        }

        if (kind === "date" || kind === "tag") {
            // the date and tag controls read their name, format and initial value from
            // their host element, so the attributes are set before they are constructed
            const host = document.createElement("div");
            host.setAttribute("name", name);
            host.setAttribute("data-value", value);

            if (kind === "date") {
                host.setAttribute("data-format", opts.format || "yyyy-MM-dd");
                host._wx_controller = new webexpress.webui.InputDateCtrl(host);
            } else {
                host._wx_controller = new webexpress.webui.InputTagCtrl(host);
            }

            return host;
        }

        const input = document.createElement("input");
        input.type = kind === "numeric" ? "number" : "text";
        input.className = "form-control";
        input.name = name;
        input.value = value;

        if (opts.placeholder) {
            input.placeholder = opts.placeholder;
        }

        return input;
    };

    /**
     * Reads the value the editor of a cell currently holds.
     *
     * The save event carries a value of its own, but the SmartEditCtrl takes it from the
     * editor control's state rather than from the editor's input, and a composite control
     * — the date picker, the tag field — only moves typed input into its state on its own
     * events. Its reported value is therefore the one from before the edit, while the
     * value the control then applies to the cell is the one read here. Reading the same
     * source keeps what is stored and what is shown in agreement.
     *
     * @param {HTMLElement} container The cell container.
     * @param {string} kind The editor kind.
     * @param {*} fallback The value the save event reported.
     * @returns {*} The value to store.
     */
    const readEditorValue = (container, kind, fallback) => {
        const form = container.querySelector("form");
        const editor = form ? form.firstElementChild : null;

        if (!editor) {
            return fallback;
        }

        if (editor.tagName === "SELECT" || editor.tagName === "INPUT" || editor.tagName === "TEXTAREA") {
            return editor.value;
        }

        // the tag field keeps its tags in the control, not in a single input
        if (kind === "tag") {
            const ctrl = editor._wx_controller;
            return ctrl && typeof ctrl.value !== "undefined" ? ctrl.value : fallback;
        }

        const inner = editor.querySelector("input");

        return inner ? inner.value : fallback;
    };

    /**
     * Brings the value an editor reports into the shape the object endpoint stores.
     * @param {*} value The value the editor reported.
     * @param {string} kind The editor kind.
     * @returns {string} The payload value.
     */
    const toPayloadValue = (value, kind) => {
        if (Array.isArray(value)) {
            value = value.join(";");
        }

        const text = value === null || typeof value === "undefined" ? "" : String(value);

        // the tag control speaks semicolons, the value rows are stored comma-separated
        return kind === "tag"
            ? text.split(/[,;]/).map((t) => t.trim()).filter((t) => t.length > 0).join(",")
            : text;
    };

    webexpress.webui.TableTemplates.register("kleenestar-cell", (val, table, row, cell, name, opts) => {
        opts = opts || {};

        const editable = opts.editable === true || opts.editable === "true";
        const action = resolveAction(row, opts);

        if (!editable || !action || !name || !isWritable(row, name)) {
            return renderReadOnly(val, table, row, cell, name, opts);
        }

        const container = document.createElement("div");
        container.className = "wx-kleenestar-cell-edit";
        container.setAttribute(ACTION_ATTRIBUTE, action);
        container.setAttribute(NAME_ATTRIBUTE, name);
        container.setAttribute(KIND_ATTRIBUTE, opts.kind || "text");
        container.appendChild(buildEditor(val, name, opts));

        // SmartEditCtrl takes over the container: it detaches the editor, shows the
        // read-only view and swaps the editor back in on a double click or on the pencil
        new webexpress.webui.SmartEditCtrl(container);

        return container;
    });

    /**
     * Stores a finished inline edit and re-queries the table it was made in.
     *
     * The re-query is what makes the cell honest: the SmartEditCtrl has already put the
     * new value on screen by the time this runs, so a rejected write would otherwise
     * leave the table showing something the server never accepted. It also refreshes the
     * columns the server derives, such as the update stamp. The endpoint announces the
     * change on its data domain as well and the table reloads on that announcement by
     * itself; a re-query that arrives twice is harmless, because a newer query supersedes
     * the one in flight.
     */
    document.addEventListener(webexpress.webui.Event.SAVE_INLINE_EDIT_EVENT, async (event) => {
        const detail = event.detail || {};
        const container = detail.sender;

        if (!container || typeof container.getAttribute !== "function") {
            return;
        }

        const action = container.getAttribute(ACTION_ATTRIBUTE);
        const name = container.getAttribute(NAME_ATTRIBUTE);

        if (!action || !name) {
            return;
        }

        const kind = container.getAttribute(KIND_ATTRIBUTE);
        const payload = {};
        payload[name] = toPayloadValue(readEditorValue(container, kind, detail.value), kind);

        try {
            const response = await fetch(action, {
                method: "PUT",
                headers: { "Content-Type": "application/json", "Accept": "application/json" },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                console.error("inline edit rejected", response.status, action, payload);
            }
        } catch (error) {
            console.error("inline edit failed", error);
        }

        // the table is found by walking up to the nearest element that carries a table
        // controller: the "wx-webapp-table" marker class is removed from the host the
        // moment the controller mounts, so a selector for it never matches afterwards
        const ctrl = webexpress.webui.Controller.getClosestInstance(container, webexpress.webapp.TableCtrl);

        if (ctrl && typeof ctrl.update === "function") {
            ctrl.update();
        }
    });
})();
