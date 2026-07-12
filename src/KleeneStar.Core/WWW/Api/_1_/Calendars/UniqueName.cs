using KleeneStar.Core.WebManager;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using System.Linq;
using System.Text.RegularExpressions;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WWW.Api._1_.Calendars
{
    using Calendar = KleeneStar.Model.Entities.Calendar;

    /// <summary>
    /// Validates whether a candidate calendar name is available within the system.
    /// </summary>
    [Title("Calendars")]
    [Cache]
    public sealed partial class UniqueName : RestApiUnique
    {
        /// <summary>
        /// Matches names of 1 to 128 non-control Unicode characters. Used to reject
        /// inputs with control characters, line breaks, or overly long values before
        /// the database is consulted.
        /// </summary>
        /// <returns>The compiled regular expression.</returns>
        [GeneratedRegex(@"^[\P{C}]{1,128}$")]
        private static partial Regex NameRegex();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public UniqueName()
        {
        }

        /// <summary>
        /// Determines whether the supplied candidate calendar name is available. A name
        /// is rejected when it is empty/whitespace, when it equals one of the
        /// <see cref="CalendarManager.ReservedCalendarNames"/> URL segments, when it
        /// contains characters disallowed by <see cref="NameRegex"/>, or when a calendar
        /// with the same name already exists (case-insensitive comparison).
        /// </summary>
        /// <param name="value">The candidate calendar name.</param>
        /// <param name="request">The HTTP request providing additional context.</param>
        /// <returns><c>true</c> when the name is available; <c>false</c> otherwise.</returns>
        protected override bool CheckAvailable(string value, Request request)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (CalendarManager.ReservedCalendarNames.Contains(value.Trim().ToLower()))
            {
                return false;
            }

            if (!NameRegex().IsMatch(value))
            {
                return false;
            }

            var query = new Query<Calendar>()
                .WhereEqualsIgnoreCase(x => x.Name, value);

            using var context = ModelHub.CreateDbContext();

            return CoreHub.CalendarManager?.GetCalendars(query, context).Any() != true;
        }
    }
}
