using KleeneStar.Model;
using System.Linq;
using System.Text.RegularExpressions;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Groups
{
    /// <summary>
    /// Validates uniqueness of group names.
    /// </summary>
    [Title("Groups")]
    [Cache]
    public sealed partial class UniqueName : RestApiUnique
    {
        /// <summary>
        /// Regex for valid names.
        /// </summary>
        [GeneratedRegex(@"^[\P{C}]{1,64}$")]
        private static partial Regex NameRegex();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public UniqueName()
        {
        }

        /// <summary>
        /// Checks name availability.
        /// </summary>
        protected override bool CheckAvailable(string value, Request request)
        {
            if (!NameRegex().IsMatch(value))
            {
                return false;
            }

            var query = new Query<Model.Entities.Group>()
                .WhereEqualsIgnoreCase(x => x.Name, value);

            using var context = ModelHub.CreateDbContext();

            return CoreHub.GroupManager?.GetGroups(query, context)
                .Any() != true;
        }
    }
}
