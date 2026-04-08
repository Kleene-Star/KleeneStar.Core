using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Statuses
{
    /// <summary>
    /// Provides functionality to retrieve WQL data.
    /// </summary>
    [Cache]
    public sealed class Wql : RestApiWqlPrompt<Model.Entities.Status>
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
        /// <remarks>
        /// Override this method in a derived class to provide custom history retrieval
        /// logic.
        /// </remarks>
        /// <param name="request">
        /// The request for which to retrieve history. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of strings representing the history entries for the 
        /// specified request. The collection is empty if no history is available.
        /// </returns>
        protected override IEnumerable<string> GetHistory(IRequest request)
        {
            yield return "Summary ~ \"Login Flow\"";
            yield return "Key ~ \"SD-39008\"";
        }
    }
}
