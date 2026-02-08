using KleeneStar.Core.WebManager;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Workspaces
{
    /// <summary>
    /// Represents a unique workspace name within the system, providing functionality to validate 
    /// and check the availability of workspace names.
    /// </summary>
    [Title("Workspace")]
    [Cache]
    public sealed partial class UniqueName : RestApiUnique
    {
        /// <summary>
        /// Provides a regular expression that matches keys consisting of 1 to 64 non-control
        /// Unicode characters.
        /// </summary>
        [GeneratedRegex(@"^[\P{C}]{1,64}$")]
        private static partial Regex KeyRegex();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public UniqueName()
        {
        }

        /// <summary>
        /// Determines whether the specified value is available based on the provided request context.
        /// </summary>
        /// <param name="value">
        /// The value to check for availability.
        /// </param>
        /// <param name="request">
        /// The request context containing additional information for the availability check. 
        /// </param>
        /// <returns>True if the specified value is available; otherwise, false.</returns>
        protected override bool CheckAvailable(string value, Request request)
        {
            if (WorkspaceManager.ReservedWorkspaceKeys.Contains(value?.Trim().ToLower()))
            {
                return false;
            }

            if (!KeyRegex().IsMatch(value))
            {
                return false;
            }

            var query = new Query<Workspace>()
                .WhereEqualsIgnoreCase(x => x.Name, value);

            using var context = ModelHub.CreateDbContext();

            return CoreHub.WorkspaceManager?.GetWorkspaces(query, context)
                .Any() != true;
        }
    }
}
