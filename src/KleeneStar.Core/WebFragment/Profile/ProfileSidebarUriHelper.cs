using System;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Profile
{
    /// <summary>
    /// Shared helper used by all profile sidebar links to determine whether the link points
    /// at the page currently being rendered.
    /// </summary>
    internal static class ProfileSidebarUriHelper
    {
        /// <summary>
        /// Determines whether the target URI matches the current request URI.
        /// </summary>
        /// <param name="renderContext">
        /// The render control context containing the current request information.
        /// </param>
        /// <param name="targetUri">
        /// The target URI to compare against the current request.
        /// </param>
        /// <returns>
        /// True if the current request URI matches the target URI (case-insensitive); 
        /// otherwise, false.
        /// </returns>
        public static bool IsActive(IRenderControlContext renderContext, IUri targetUri)
        {
            var current = string.Join("/", renderContext.Request.Uri.PathSegments ?? []);
            var target = string.Join("/", targetUri.BindParameters(renderContext.Request).PathSegments ?? []);

            return string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        }
    }
}
