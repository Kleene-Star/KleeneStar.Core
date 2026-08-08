using System;
using System.IO;
using System.Linq;
using System.Text;

namespace KleeneStar.Core.WebFragment.Object.Issues
{
    /// <summary>
    /// Builds the inline cell renderer script of the object overview tables from the
    /// embedded <c>Assets/js/tableinlineedit.js</c>.
    /// </summary>
    /// <remarks>
    /// The script is emitted inline into the page head rather than linked as a static
    /// asset, for the same reason the dashboard widget script is (see
    /// <see cref="Dashboard.DashboardWidgetScript"/>): the core plugin's own embedded
    /// assets would mount under <c>/{app}/assets/…</c>, a route the application already
    /// owns for its workspace-assets feature, so a link to them answers 404 and the
    /// renderer would never register — leaving every cell as plain text with no editor.
    /// </remarks>
    internal static class IssueTableInlineEditScript
    {
        /// <summary>
        /// The suffix of the embedded script resource, matched against the manifest
        /// resource names with their directory separators normalized.
        /// </summary>
        private const string ScriptResourceSuffix = "js/tableinlineedit.js";

        /// <summary>
        /// The inline script, read once on first use.
        /// </summary>
        private static readonly Lazy<string> Script = new(Build);

        /// <summary>
        /// Gets the inline cell renderer script, or an empty string when the resource
        /// could not be read.
        /// </summary>
        public static string Value => Script.Value;

        /// <summary>
        /// Reads the embedded script resource.
        /// </summary>
        /// <returns>The script, or an empty string when the resource could not be read.</returns>
        private static string Build()
        {
            var assembly = typeof(IssueTableInlineEditScript).Assembly;
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
