using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Slas
{
    /// <summary>
    /// Provides WQL prompt suggestions for the SLA-policy advanced search.
    /// </summary>
    [Cache]
    public sealed class Wql : RestApiWqlPrompt<SlaPolicy>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Wql()
        {
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetHistory(IRequest request)
        {
            yield return "Name ~ \"VIP\"";
            yield return "State = Active";
        }
    }
}
