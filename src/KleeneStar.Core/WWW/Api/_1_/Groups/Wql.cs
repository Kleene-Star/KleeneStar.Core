using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Groups
{
    /// <summary>
    /// Provides WQL search functionality for groups.
    /// </summary>
    [Cache]
    public sealed class Wql : RestApiWqlPrompt<Model.Entities.Group>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Wql()
        {
        }

        /// <summary>
        /// Retrieves search history entries.
        /// </summary>
        protected override IEnumerable<string> GetHistory(IRequest request)
        {
            yield return "Name ~ \"Administrators\"";
            yield return "Name ~ \"Developers\"";
        }
    }
}
