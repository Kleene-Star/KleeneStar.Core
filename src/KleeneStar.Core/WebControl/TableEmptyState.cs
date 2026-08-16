using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Core.WebControl
{
    /// <summary>
    /// Gives a REST-backed table the empty-state placeholder the control does not offer.
    /// </summary>
    /// <remarks>
    /// <see cref="WebExpress.WebApp.WebControl.ControlDataTable"/> paints its rows on the
    /// client, so an empty result renders as a header with nothing under it, and neither the
    /// control nor <c>RestApiTableResult</c> carries a message to put there instead. The
    /// placeholder is therefore authored here as a sibling element the table names, and the
    /// companion script (<c>Assets/js/tableemptystate.js</c>) toggles it on the table's own
    /// <c>data arrived</c> event.
    /// <para>
    /// The text is resolved server-side at render time, so it is localized like every other
    /// label and the script stays free of any wording.
    /// </para>
    /// </remarks>
    public static class TableEmptyState
    {
        /// <summary>
        /// The suffix of the embedded script resource, matched against the manifest resource
        /// names with their directory separators normalized.
        /// </summary>
        private const string ScriptResourceSuffix = "js/tableemptystate.js";

        /// <summary>
        /// The inline script, read once on first use.
        /// </summary>
        private static readonly Lazy<string> _script = new(Build);

        /// <summary>
        /// Gets the script that toggles the placeholders, or an empty string when the resource
        /// could not be read.
        /// </summary>
        /// <remarks>
        /// It is emitted inline into the page head rather than linked as a static asset, for
        /// the same reason the dashboard widget script is: the plugin's embedded assets would
        /// mount under <c>/{app}/assets/…</c>, a route the application already owns, so a link
        /// to them answers 404 and the placeholder would never appear.
        /// </remarks>
        public static string Script => _script.Value;

        /// <summary>
        /// Builds the placeholder that is shown while a table has no rows.
        /// </summary>
        /// <remarks>
        /// The element starts hidden, so a table that never loads — an endpoint that is down,
        /// a request that is still in flight — shows nothing rather than claiming emptiness.
        /// </remarks>
        /// <param name="tableId">The id of the table control the placeholder belongs to.</param>
        /// <param name="messageResource">The resource key of the message to show.</param>
        /// <returns>The placeholder control.</returns>
        public static IControl Create(string tableId, string messageResource)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableId);

            var head = new StringBuilder()
                .Append("<div class=\"ks-table-empty wx-color-secondary\" data-ks-empty-for=\"")
                .Append(WebUtility.HtmlEncode(tableId))
                .Append("\" hidden>")
                .ToString();

            return new ControlHtml($"{tableId}-empty")
            {
                // the message is translated per request rather than once at construction, so a
                // cached fragment does not freeze the first visitor's language onto the page
                Html = renderContext => head
                    + WebUtility.HtmlEncode(I18N.Translate(renderContext, messageResource))
                    + "</div>"
            };
        }

        /// <summary>
        /// Reads the embedded script resource.
        /// </summary>
        /// <returns>The script, or an empty string when the resource could not be read.</returns>
        private static string Build()
        {
            var assembly = typeof(TableEmptyState).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(name => name.Replace('\\', '/')
                    .EndsWith(ScriptResourceSuffix, StringComparison.OrdinalIgnoreCase));

            if (resourceName is null)
            {
                return string.Empty;
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream is null)
            {
                return string.Empty;
            }

            using var reader = new StreamReader(stream, Encoding.UTF8);

            return reader.ReadToEnd();
        }
    }
}
