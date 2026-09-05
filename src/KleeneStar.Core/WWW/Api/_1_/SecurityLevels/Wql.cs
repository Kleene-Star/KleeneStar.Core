using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.SecurityLevels
{
    /// <summary>
    /// Provides functionality to retrieve WQL data for security levels.
    /// </summary>
    [Cache]
    public sealed class Wql : RestApiWqlPrompt<Model.Entities.SecurityLevel>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Wql()
        {
        }

        /// <summary>
        /// Retrieves a collection of historical entries associated with the specified request.
        /// </summary>
        /// <param name="request">The request for which to retrieve history. Cannot be null.</param>
        /// <returns>The history entries, empty when none is available.</returns>
        protected override IEnumerable<string> GetHistory(IRequest request)
        {
            yield return "Name ~ \"Confidential\"";
            yield return "Rank > 0";
        }
    }
}
