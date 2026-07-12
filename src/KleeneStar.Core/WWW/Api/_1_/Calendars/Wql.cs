using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Calendars
{
    using Calendar = KleeneStar.Model.Entities.Calendar;

    /// <summary>
    /// Provides WQL prompt suggestions for the calendar advanced search.
    /// </summary>
    [Cache]
    public sealed class Wql : RestApiWqlPrompt<Calendar>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Wql()
        {
        }

        /// <summary>
        /// Returns the WQL expressions that pre-populate the advanced-search prompt
        /// suggestion list. These are pre-canned starter queries the user can refine
        /// without learning WQL syntax from scratch.
        /// </summary>
        /// <param name="request">The HTTP request providing additional context.</param>
        /// <returns>The seeded WQL history entries.</returns>
        protected override IEnumerable<string> GetHistory(IRequest request)
        {
            yield return "Name ~ \"Standard\"";
            yield return "TimeZone = \"Europe/Berlin\"";
            yield return "IsDefault = true";
        }
    }
}
