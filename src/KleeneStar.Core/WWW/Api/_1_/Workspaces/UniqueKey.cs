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
    [Title("Workspace")]
    [Cache]
    public sealed partial class UniqueKey : RestApiUnique
    {
        /// <summary>
        /// Provides a compiled regular expression that matches strings containing only alphanumeric 
        /// characters (letters
        /// and digits).
        /// </summary>
        [GeneratedRegex("^[a-zA-Z0-9]{1,10}$")]
        private static partial Regex KeyRegex();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public UniqueKey()
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
                .WhereStartsWithIgnoreCase(x => x.Key, value);

            using var contet = ModelHub.CreateDbContext();

            return CoreHub.WorkspaceManager?.GetWorkspaces(query, contet)
                .Any() != true;
        }
    }
}
