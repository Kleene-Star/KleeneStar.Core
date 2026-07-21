using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

// The entity type Object collides with System.Object; alias it so the
// prompt type argument reads naturally.
using ObjectEntity = KleeneStar.Model.Entities.Object;

namespace KleeneStar.Core.WWW.Api._1_.Assets._workspacekey_
{
    /// <summary>
    /// Advanced-search prompt endpoint of the asset overview. The search control
    /// fetches its history and lookahead suggestions from this endpoint while the
    /// plain-text query is forwarded to the <see cref="Table"/> endpoint as the
    /// <c>q</c> parameter.
    /// </summary>
    [Cache]
    public sealed class Wql : RestApiWqlPrompt<ObjectEntity>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Wql()
        {
        }

        /// <summary>
        /// Provides a small set of example queries shown in the prompt history.
        /// </summary>
        /// <param name="request">The request for which to retrieve history.</param>
        /// <returns>The example query history entries.</returns>
        protected override IEnumerable<string> GetHistory(IRequest request)
        {
            yield return "Summary ~ \"Laptop\"";
            yield return "Key ~ \"SD\"";
        }
    }
}
